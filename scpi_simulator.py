from logging import exception
import socket
import sys
import time

def scpi_simulator():
    # Create a TCP/IP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Bind the socket to the port
    server_address = ('127.0.0.1', 5025)
    print('starting up on %s port %s' % server_address)
    print('VISA ADDRESS: TCPIP0::%s::%s::SOCKET' % (server_address[0], server_address[1]))
    
    sock.bind(server_address)

    # Listen for incoming connections
    sock.listen(1)

    while True:
        # Wait for a connection
        print('waiting for a connection')
        try:
            connection, client_address = sock.accept()
            print('connection from', client_address)

            # Receive the data in larger chunks and retransmit it
            while True:
                data = connection.recv(1024)  # Increased buffer size to 1024 bytes
                print('received "%s"' % data.decode().replace('\n', '\\n'))
                if data:
                    command = data.decode().strip().upper()
                    if command == 'EXIT':
                        print('Exiting the simulator...')
                        return
                    elif command == '*IDN?':
                        response = "Simulator,SCPI,1.0\n"
                        connection.sendall(response.encode())
                    elif command == '*OPC?':
                        print('Waiting 5 seconds before responding...')
                        time.sleep(5)  # Wait for 5 seconds
                        response = "1\n"
                        connection.sendall(response.encode())
                    else:
                        response = "Unknown command\n"
                        connection.sendall(response.encode())
                else:
                    print('no data from', client_address)
                    break
        except ConnectionResetError:
            print("Connection reset by user")
        finally:
            # Clean up the connection
            connection.close()

# Example usage
if __name__ == "__main__":
    try:
        scpi_simulator()
    except KeyboardInterrupt:
        print("\nExiting the simulator...")
        sys.exit()
