package buri.momserver;

import java.io.IOException;

/**
 * MomServer main class
 *
 * @author belabursan
 */
public final class MomServer {

    static final String VERSION = "0.0.1";
    private Server server;

    private void close() throws InterruptedException {
        if (server != null) {
            server.closeServer();
            server = null;
        }
    }

    private synchronized void shutDown() {
        notifyAll();
    }

    private synchronized void run(String[] args) throws InterruptedException {
        try {
            registerShutDownHook();
            server = Server.getInstance(args.length > 0 ? args[0] : ServerProperties.PROPERTY_FILE_NAME);
            server.startServer();
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
