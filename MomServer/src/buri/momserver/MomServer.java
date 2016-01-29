package buri.momserver;

import java.io.File;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * MomServer main class
 *
 * @author belabursan
 */
public final class MomServer {

    static final String VERSION = "0.0.4";
    private ServerCore serverCore;
    private Thread shutDownThread;
    private final MomLogger log;
    
    public MomServer() throws SecurityException, IOException{
        this.log = MomLogger.getLogger();
    }

    /**
     * Signals the wait in the run() method. Only called by the shutdown hook!
     */
    private synchronized void shutDown() {
        log.info(" -got shutdown signal");
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
            log.debug("starting to execute server");
            serverCore = ServerCore.getInstance(arguments.getPropertyFilePath());
            if (serverCore.startServer()) {
                System.out.println("MomServer v" + VERSION + " started, exit with Ctrl+C");
                if (arguments.isDaemon()) {
                    log.warning("server will run as daemon");
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
            log.error("Intrrupted exception: " + ix.getMessage());
        } catch (IOException ex) {
            log.error("IO exception: " + ex.getMessage());
        } finally {
            awaitShutdownHook();
        }
    }

    /**
     * Awaits the termination of the shut down thread and unregisters it.
     *
     * @throws InterruptedException
     */
    private void awaitShutdownHook() throws InterruptedException {
        log.info("awaiting shut down hook");
        if (shutDownThread.getState() != Thread.State.NEW) {
            log.debug("shutdown hook startat, now trying to join");
            shutDownThread.join();
        }
    }

    /**
     * Creates a shutdown hook the hook will call the shutDown() method at
     * shutdown
     */
    private void registerShutDownHook() {
        log.info("registering shut down hook");
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
            CLArguments arguments = CLArguments.resolveArguments(args);
            if (arguments.isDebug()) {
                MomLogger.initLogger(Level.FINEST);
            }
            MomLogger log = MomLogger.getLogger();
            log.error("Starting MomServer v" + VERSION);
            final MomServer mom = new MomServer();
            mom.execute(arguments);
        } catch (InterruptedException ex) {
            System.out.println("Main thread interrupted, exiting! (" + ex.getMessage() + ")");
        } catch (IllegalArgumentException ix) {
            System.out.println("Failed to start the server: " + ix.getMessage());
        } catch (SecurityException | IOException ex) {
            System.out.println("Could not start logger: " + ex.getMessage());
        } finally {
            MomLogger.closeLogger();
        }
        System.exit(0);
    }
}
