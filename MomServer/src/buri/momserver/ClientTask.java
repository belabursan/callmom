package buri.momserver;

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

    /**
     * Creates a new instance of the client task
     *
     * @param socket socket connected to the Internet to save
     */
    private ClientTask(Socket socket) {
        this.socket = socket;
        LOG.log(Level.FINEST, "new client task created");
    }

    /**
     * Returns a new instance of a client task
     *
     * @param socket connected socket
     * @return ClientTask
     */
    static ClientTask newInstance(final Socket socket) {
        return new ClientTask(socket);
    }

    /**
     * Runs the client task, called by a client thread from the thread pool
     */
    @Override
    public void run() {
        IClient client = null;
        int retValue = IClient.SUCCESS;
        try {
            client = new Client();
            retValue = client.main(socket);
        } catch (Exception ex) {
            //catch everything here, we don't want the client exceptions in the server!!!
            LOG.log(Level.SEVERE, "client finished with exception: {0}",
                    ex.getLocalizedMessage());
            LOG.throwing(this.getClass().getSimpleName(), "run()", ex);
        } finally {
            //end the client and print logs
            handleLogsAndClose(retValue, client);
        }
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
                if (retValue != IClient.SUCCESS) {
                    Collection<String> logs = client.getLogs();

                    //alternatively create new file?
                    if (logs != null && !logs.isEmpty()) {
                        for (String s : logs) {
                            LOG.log(Level.WARNING, "Client: {0}", s);
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
