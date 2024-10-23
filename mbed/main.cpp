#include "mbed.h"
#include "ESP8266Interface.h"
#include "TCPSocket.h"
#include "string.h"

#define REMOTE_PORT 5000
#define REMOTE_ADDRESS "192.168.7.209"
//#define REMOTE_ADDRESS "192.168.68.11"
#define BUFFER_SIZE 128
#define DATA_SENDING_DELAY 500ms

// SPI for Pmod ACL2
SPI spi(PB_5, PA_11, PB_3); // MOSI, MISO, SCK
DigitalOut cs(PB_4);        // Chip Select for SPI

// WiFi setup
ESP8266Interface esp(MBED_CONF_APP_ESP_TX_PIN, MBED_CONF_APP_ESP_RX_PIN);

// Socket for TCP communication
TCPSocket socket;

// Pmod ACL2 Initialization
void initAccelerometer() {
    cs = 1;
    spi.format(8, 3);  // 8-bit data, Mode 3
    spi.frequency(1000000);  // 1 MHz clock

    // Step 1: Put ADXL362 into measurement mode
    cs = 0;
    spi.write(0x0A);  // Write command
    spi.write(0x2D);  // Power control register
    spi.write(0x02);  // Measurement mode (bit 1 set to 1)
    cs = 1;

    // Step 2: Set output data rate (ODR) to 100 Hz (default is 12.5 Hz)
    cs = 0;
    spi.write(0x0A);  // Write command
    spi.write(0x2C);  // Filter control register
    spi.write(0x03);  // Set ODR to 100 Hz (bits 0-3)
    cs = 1;

    // Step 3: Wait for configuration to take effect
    ThisThread::sleep_for(100ms);
}

// Pmod ACL2 Reading Function
int16_t readAcceleration(uint8_t reg) {
    cs = 0;
    spi.write(0x0B);  // Read command
    spi.write(reg);   // Register address
    uint8_t lowByte = spi.write(0x00);  // Read low byte
    uint8_t highByte = spi.write(0x00); // Read high byte
    cs = 1;

    int16_t result = (highByte << 8) | lowByte;  // Combine low and high byte
    if (result & 0x800) {  // Sign extend the 12-bit data if negative
        result |= 0xF000;
    }
    return result;
}

// Function to connect to the TCP server
bool connectToServer(TCPSocket &socket, const SocketAddress &server) {
    socket.close();  // Close any existing connection
    socket.open(&esp);
    int result = socket.connect(server);
    if (result == 0) {
        printf("Succesfully connected to %s:%d\n", server.get_ip_address(), server.get_port());
        return true;
    } else {
        printf("Failed the connection to %s:%d, error: %d\nRetrying in 5 seconds...\n", server.get_ip_address(), server.get_port(), result);
        return false;
    }
}

int main() {
    // Initialize the accelerometer
    initAccelerometer();

    // Connect to WiFi
    printf("\r\nConnecting to WiFi...\r\n");
    int ret = esp.connect(MBED_CONF_APP_WIFI_SSID, MBED_CONF_APP_WIFI_PASSWORD, NSAPI_SECURITY_WPA_WPA2);
    if (ret != 0) {
        printf("Connection error: %d\n", ret);
        return -1;
    }

    // Display WiFi connection details
    SocketAddress espAddress;
    printf("Success\n");
    printf("MAC: %s\n", esp.get_mac_address());
    esp.get_ip_address(&espAddress);
    printf("IP: %s\n", espAddress.get_ip_address());
    printf("Netmask: %s\n", esp.get_netmask());
    printf("Gateway: %s\n", esp.get_gateway());
    printf("RSSI: %d\n", esp.get_rssi());

    // Set up the server address
    // These should match the IP and the port of the PC that is running the Unity client
    // The port is specified in the Unity Editor inspector as a int value, by default set to '5000'
    SocketAddress server(REMOTE_ADDRESS, REMOTE_PORT);

    // Attempt to connect to the server
    bool connected = connectToServer(socket, server);

    // Main loop
    while (true) {
        if (!connected) {
            // Try reconnecting every 5 seconds if not connected
            ThisThread::sleep_for(5s);
            connected = connectToServer(socket, server);
            continue;
        }

        // Read X, Y, and Z axis data from the accelerometer
        int16_t xAccel = readAcceleration(0x0E);  // X-axis data
        int16_t yAccel = readAcceleration(0x10);  // Y-axis data
        int16_t zAccel = readAcceleration(0x12);  // Z-axis data

        // Format the accelerometer data into a string
        char buffer[BUFFER_SIZE];
        snprintf(buffer, sizeof(buffer), "X:%d,Y:%d,Z:%d", xAccel, yAccel, zAccel);

        // Send the formatted data over the TCP connection
        int sent_bytes = socket.send(buffer, strlen(buffer));

        if (sent_bytes >= 0) {
            printf("Sent: %s\n", buffer);
        } else {
            printf("Error sending data: %d\n", sent_bytes);
            connected = false;  // Mark as disconnected
            socket.close();  // Close the socket to reset the connection
        }

        // Wait for the specified duration before sending the next set of data
        ThisThread::sleep_for(DATA_SENDING_DELAY);
    }
}
