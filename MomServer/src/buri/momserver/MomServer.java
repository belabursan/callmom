package buri.momserver;

import java.io.File;
import java.io.IOException;

/**
 * MomServer main class
 *
 * @author belabursan
 */
public final class MomServer {

    static final String VERSION = "0.0.4";
    private ServerCore serverCore;
    private Thread shutDownThread;

    /**
     * Signals the wait in the run() method. Only called by the shutdown hook!
     */
    private synchronized void shutDown() {
        System.out.println(" -got shutdown signal");
        if (serverCore != null) {
            serverCore.closeServer();
            serverCore = null;
        }
        notifyAll();
    }

    /**
     * Starts and runs the server, blocks until a shutdown is initialized and
     * then closes the server.
     *
     * @param args command line arguments, holds an alternative path to the
     * property file
     * @throws InterruptedException if interrupted during the close process
     */
    private synchronized void execute(final CLArguments arguments) throws InterruptedException {
        registerShutDownHook();
        try {
            serverCore = ServerCore.getInstance(arguments.getPropertyFilePath());
            if (serverCore.startServer()) {
                System.out.println("MomServer v" + VERSION + " started, exit with Ctrl+C");
                if (arguments.isDaemon()) {
                    System.out.println("IS DAEMON");
                    File pidfile = new File(System.getProperty("daemon.pidfile"));
                    pidfile.deleteOnExit();
                    System.out.close();
                    System.err.close();
                }
                //wait until the shut down hook is activated
                wait();
            }
        } catch (InterruptedException ix) {
            System.out.println("Interrupted, exiting: " + ix.getMessage());
        } catch (IOException ex) {
            System.out.println("Could not start the server: " + ex.getMessage());
        } finally {
            unregisterShutdownHook();
        }
    }

    /**
     * Awaits the termination of the shut down thread and unregisters it.
     *
     * @throws InterruptedException
     */
    private void unregisterShutdownHook() throws InterruptedException {
        System.out.println(" -unregistering shutdown thread");
        try {
            if (shutDownThread.getState() != Thread.State.NEW) {
                shutDownThread.join();
            }
            shutDownThread.interrupt();
        } finally {
            Runtime.getRuntime().removeShutdownHook(shutDownThread);
            shutDownThread = null;
        }
    }

    /**
     * Creates a shutdown hook the hook will call the shutDown() method at
     * shutdown
     */
    private void registerShutDownHook() {
        shutDownThread = new Thread() {
            @Override
            public void run() {
                Thread.currentThread().setName("ShutDownThread");
                shutDown();
            }
        };
        Runtime.getRuntime().addShutdownHook(shutDownThread);
    }

    /**
     * Main method, everything starts and ends here
     *
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        Thread.currentThread().setName("MainThread");
        System.out.println("");
        try {
            final MomServer mom = new MomServer();
            mom.execute(CLArguments.resolveArguments(args));
        } catch (InterruptedException ex) {
            System.out.println("Main thread interrupted, exiting! (" + ex.getMessage() + ")");
        } catch (IllegalArgumentException ix) {
            System.out.println("Failed to start the server: " + ix.getMessage());
        }
        System.out.println("exiting now");
        System.exit(0);
    }
}
