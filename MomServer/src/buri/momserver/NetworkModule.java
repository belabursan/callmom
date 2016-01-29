package buri.momserver;

import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.net.ServerSocketFactory;
import javax.net.ssl.SSLServerSocketFactory;

/**
 * Communication module, starts a server socket on the defined port
 *
 * @author Bela Bursan
 */
final class NetworkModule {

    private final int port, nrOfMaxClients;
    private final boolean reuseAddress;
    private ServerSocket servSoc;
    private static NetworkModule network = null;
    private static final Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);

    /**
     * Creates a new communication module
     *
     * @param port port to listen on
     * @param nrOfMaxClients maximum number of clients to allow connection at
     * the same time
     * @param reuseAddress sets the reuse address option of the socket
     */
    private NetworkModule(int port, int nrOfMaxClients, boolean reuseAddress) {
        this.port = port;
        this.nrOfMaxClients = nrOfMaxClients;
        this.reuseAddress = reuseAddress;
        LOG.finest("network module created");
    }

    /**
     * Creates a new communication module
     *
     * @param port port to listen on
     * @param nrOfMaxClients maximum number of clients to allow connection at
     * the same time
     * @param reuseAddress sets the reuse address option of the socket
     */
    static NetworkModule getInstance(int port, int nrOfMaxClients, boolean reuseAddress) {
        if (network == null) {
            network = new NetworkModule(port, nrOfMaxClients, reuseAddress);
        }
        return network;
    }

    /**
     * Creates a server socket, bound to the specified port.
     *
     * @throws SocketException if the reuse address option cannot be set
     * @throws IOException if the socket cannot be created
     */
    void startTCPServer() throws SocketException, IOException {
        servSoc = new ServerSocket(port, nrOfMaxClients);
        servSoc.setReuseAddress(reuseAddress);
        LOG.log(Level.FINEST, "network module started, socket listening on port {0}", port);
    }

    /**
     * Starts a SSL server socket
     *
     * @throws SocketException if the reuse address option cannot be set
     * @throws IOException if the socket cannot be created
     */
    void startSSLServer(boolean reuseAddress) throws SocketException, IOException {
        ServerSocketFactory ssocketFactory = SSLServerSocketFactory.getDefault();
        servSoc = ssocketFactory.createServerSocket(port);
        servSoc.setReuseAddress(this.reuseAddress);
        LOG.log(Level.FINEST, "network module started, SSL socket listening on port {0}", port);
    }

    /**
     * Listens for a connection to be made to this socket and accepts it. The
     * method blocks until a connection is made.
     *
     * @return a new, connected java.net.Socket
     * @throws java.io.IOException in case of the server socket is being shut
     * down
     */
    Socket getSocket() throws IOException {
        return servSoc.accept();
    }

    /**
     * Closes the server socket
     */
    void close() {
        LOG.fine("closing network module");
        if (servSoc != null) {
            try {
                servSoc.close();
                servSoc = null;
            } catch (IOException ex) {
                LOG.log(Level.WARNING, "closing server socket ...:{0}", ex.getMessage());
            }
        }
        network = null;
    }

    /**
     * Returns the local port as integer
     *
     * @return Returns the local port as integer
     */
    int getServerPort() {
        return servSoc.getLocalPort();
    }

    /**
     * Returns the local IP
     *
     * @return Returns the local IP address or empty string if local address
     * cannot be read
     */
    String getServerAddress() {
        try {
            return InetAddress.getLocalHost().getHostAddress();
        } catch (UnknownHostException ex) {
            LOG.log(Level.SEVERE, "getServerAddress:unknownHostException: {0}", ex.getMessage());
        }
        return "";
    }

    /**
     * Checks if the server socket is null
     *
     * @return true if server socket is not null, false otherwise
     */
    boolean isAlive() {
        return servSoc != null;
    }

}
