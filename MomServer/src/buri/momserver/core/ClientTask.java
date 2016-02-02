package buri.momserver.core;

import buri.momserver.defaulclient.Client;
import java.net.Socket;
import java.util.Collection;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Client task, runs on the server side and communicates with a client connected
 * via the Internet.
 *
 * @author Bela Bursan
 */
final class ClientTask implements Runnable {

    private final static Logger LOG = Logger.getLogger(MomLogger.LOGGER_NAME);
    private final Socket socket;
    private final boolean isDebug;

    /**
     * Creates a new instance of the client task
     *
     * @param socket socket connected to the Internet to save
     */
    private ClientTask(Socket socket, boolean isDebug) {
        this.socket = socket;
        this.isDebug = isDebug;
        LOG.log(Level.FINEST, "new client task created");
    }

    /**
     * Returns a new instance of a client task
     *
     * @param socket connected socket
     * @return ClientTask
     */
    static ClientTask newInstance(final Socket socket, final boolean isDebug) {
        return new ClientTask(socket, isDebug);
    }

    /**
     * Runs the client task, called by a client thread from the thread pool
     */
    @Override
    public void run() {
        IClient client = null;
        int retValue = IClient.SUCCESS;
        try {
            client = getClient();
            retValue = client.execute(socket, isDebug);
        } catch (Exception ex) {
            //catch everything here, we don't want the client exceptions in the server!!!
            LOG.log(Level.SEVERE, "client finished with exception: {0}",
                    ex.getLocalizedMessage());
            //LOG.throwing(this.getClass().getName(), "run()", ex);
        } finally {
            //end the client and print logs
            handleLogsAndClose(retValue, client);
        }
    }

    /**
     * Now it returns a new instance of a default client object. In future this
     * method will decide which client to use(plug-in clients?)
     *
     * @return a new instance of a Client object
     */
    private IClient getClient() {
        //future:
        // 1: read socket
        // 2: decide which socket to use
        // 3: dinamically load appropiate Client
        // http://www.oracle.com/technetwork/articles/javase/extensible-137159.html

        return new Client();
    }

    /**
     * Closes the client and eventually prints the logs if needed
     *
     * @param retValue return value from the client
     * @param client client object
     */
    private void handleLogsAndClose(final int retValue, final IClient client) {
        try {
            if (client != null) {
                if (retValue != IClient.SUCCESS || isDebug) {
                    Collection<String> logs = client.getLogs();

                    //alternatively create new file?
                    if (logs != null && !logs.isEmpty()) {
                        for (String s : logs) {
                            LOG.log(Level.WARNING, " + Client: {0}", s);
                        }
                    }
                }
                client.close();
            }
        } catch (Exception ex) {
            LOG.log(Level.WARNING, "exception when terminating client: {0}",
                    ex.getLocalizedMessage());
        }
    }
}
