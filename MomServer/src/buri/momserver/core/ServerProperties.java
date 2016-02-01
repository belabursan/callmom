package buri.momserver.core;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Holds the server properties
 *
 * @author belabursan
 */
final class ServerProperties {

    public static final String PORT = "port";
    public static final String MAX_CLIENTS = "maxClients";
    public static final String REUSE_ADDRESS = "reuseAddress";
    public static final String DEFAULT_PROPERTY_FILE_NAME = "property.xml";
    public static final String SSL = "ssl";
    //
    private static final int DEFAULT_MAX_CLIENTS = 200;
    private static final int DEFAULT_PORT = 10888;
    private static final boolean DEFAULT_REUSE_ADDRESS = true;
    private static final boolean DEFAULT_SSL = false;
    private static final Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);

    private int port;
    private int numberOfClients;
    private boolean reuseAddress;
    private boolean ssl;

    /**
     * Returns the port number the server will listen
     *
     * @return port number without validation
     */
    int getPort() {
        return port;
    }

    /**
     * Sets the port number the server will use for connections
     *
     * @param port port to set
     */
    void setPort(int port) {
        this.port = port;
    }

    /**
     * Gets the max number of clients the server will allow to connect
     *
     * @return the max number of clients
     */
    int getNumberOfClients() {
        return numberOfClients;
    }

    /**
     * Sets the number of clients the server will allow to connect
     *
     * @param numberOfClients max number of clients to set
     */
    void setNumberOfClients(int numberOfClients) {
        this.numberOfClients = numberOfClients;
    }

    /**
     * Sets the boolean indicating if the server should reuse socket addresses
     *
     * @param reuseAddress
     */
    void setReuseAddress(boolean reuseAddress) {
        this.reuseAddress = reuseAddress;
    }

    /**
     * Gets the boolean indicating if the server should reuse socket addresses
     *
     * @return true if reuse address, false otherwise
     */
    boolean isReuseAddress() {
        return reuseAddress;
    }
    
    
    /**
     * Returns a boolean indicating if SSL network connection should be used
     * @return true if SSL is used, false otherwise
     */
    boolean isSSL() {
        return ssl;
    }

    /**
     * Sets the SSL parameter
     * @param ssl true if SSL should be used, false otherwise
     */
    private void setSSL(boolean ssl) {
        this.ssl = ssl;
    }

    /**
     * Reads properties from a file and converts it to ServerProperties object
     *
     * @param file file to read
     * @return ServerProperties object
     * @throws FileNotFoundException if the property file is not found
     * @throws IOException if the property file cannot be read
     */
    static ServerProperties readProperties(File file) throws FileNotFoundException, IOException {
        LOG.fine("reading properties");
        //load properties from the file
        Properties properties = new Properties();
        properties.loadFromXML(new FileInputStream(file));

        //convert the properties to ServerProperties object
        ServerProperties p = new ServerProperties();
        p.setPort(p.toInteger(properties.getProperty(PORT), DEFAULT_PORT));
        p.setNumberOfClients(p.toInteger(properties.getProperty(MAX_CLIENTS), DEFAULT_MAX_CLIENTS));
        p.setReuseAddress(p.toBoolean(properties.getProperty(REUSE_ADDRESS), DEFAULT_REUSE_ADDRESS));
        p.setSSL(p.toBoolean(properties.getProperty(SSL), DEFAULT_SSL));

        //TODO - add more properties here
        return p;
    }

    /**
     * Creates a new property file in the current running directory with default
     * values
     *
     * @throws FileNotFoundException
     * @throws IOException
     */
    static void createDefaultPropertyFile() throws FileNotFoundException, IOException {
        LOG.finest("creating default properties");
        Properties properties = new Properties();
        properties.put(PORT, String.valueOf(DEFAULT_PORT));
        properties.put(MAX_CLIENTS, String.valueOf(DEFAULT_MAX_CLIENTS));
        properties.put(REUSE_ADDRESS, String.valueOf(DEFAULT_REUSE_ADDRESS));
        properties.put(SSL, String.valueOf(DEFAULT_SSL));
        
        properties.storeToXML(new FileOutputStream(
                new File(DEFAULT_PROPERTY_FILE_NAME)), "Property file for the MomServer (v" + MomServer.VERSION + ")"
        );
    }

    /**
     * Converts a string to integer
     *
     * @param property string containing an integer to convert
     * @param defaultProperty default value if converting the property fails
     * @return integer value of the string argument
     * @throws NumberFormatException if nor the value or the default value is a
     * valid integer
     */
    private int toInteger(String property, int defaultProperty) {
        try {
            return Integer.parseInt(property);
        } catch (NumberFormatException | NullPointerException nx) {
            LOG.log(Level.SEVERE, "failed to parse int: {0}", nx.getMessage());
        }
        return defaultProperty;
    }

    /**
     * Converts a string to boolean
     *
     * @param property string to convert
     * @param defaultProperty default string to convert if
     * @return true if the argument is "True", case ignored, false otherwise
     */
    private boolean toBoolean(String property, boolean defaultProperty) {
        try {
            return Boolean.parseBoolean(property);
        } catch (NumberFormatException | NullPointerException nx) {
            LOG.log(Level.SEVERE, "failed to parse boolean: {0}", nx.getMessage());
        }
        return defaultProperty;
    }


}
