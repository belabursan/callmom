package buri.momserver;

import java.io.IOException;

/**
 * MomServer main class
 *
 * @author belabursan
 */
public final class MomServer {

    static final String VERSION = "0.0.2";
    private ServerCore serverCore;

    /**
     * Closes the server and terminates the execution
     *
     * @throws InterruptedException if the closing thread is interrupted by the
     * OS
     */
    private void close() throws InterruptedException {
        System.out.println("closing MomServer");
        if (serverCore != null) {
            serverCore.closeServer();
            serverCore = null;
        }
    }

    /**
     * Signals the wait in the run() method. Only called by the shutdown hook!
     */
    private synchronized void shutDown() {
        System.out.println("got shutdown signal");
        notifyAll();
    }

    /**
     * Starts and runs the server, blocks until a shutdown is initialized and
     * then closes the server
     *
     * @param args command line arguments, holds an alternative path to the
     * property file
     * @throws InterruptedException if interrupted during the close process
     */
    private synchronized void run(String[] args) throws InterruptedException {
        try {
            registerShutDownHook();
            serverCore = ServerCore.getInstance(args.length > 0 ? args[0] : ServerProperties.PROPERTY_FILE_NAME);
            serverCore.startServer();
            System.out.println("MomServer v" + VERSION + " started, exit with Ctrl+C");
            wait();

        } catch (InterruptedException ix) {
            System.out.println("Interrupted, exiting: " + ix.getMessage());
        } catch (IOException ex) {
            System.out.println("could not start the server: " + ex.getMessage());
        }

        close();
    }

    /**
     * Creates a shutdown hook the hook will call the shutDown() method at
     * shutdown
     */
    private void registerShutDownHook() {
        Runtime.getRuntime().addShutdownHook(new Thread() {
            @Override
            public void run() {
                shutDown();
            }
        });
    }

    /**
     * Main method, everything starts and ends here
     *
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        Thread.currentThread().setName("MainThread");
        try {
            final MomServer mom = new MomServer();
            mom.run(args);
        } catch (InterruptedException ex) {
            System.out.println("Main thread interrupted, exiting!");
        }
    }
}
