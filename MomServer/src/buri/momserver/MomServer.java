package buri.momserver;

import java.io.IOException;

/**
 *
 * @author belabursan
 */
public class MomServer {
    public static final String VERSION = "0.0.1";
    
    private static volatile boolean closed;

    private static void close() {
        closed = true;
        
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws IOException {
        closed = false;
        Runtime.getRuntime().addShutdownHook(new Thread() {
            @Override
            public void run() { MomServer.close(); }
        });

        ServerProperties.createDefaultPropertyFile();
        Server s = Server.getInstance(ServerProperties.PROPERTY_FILE_NAME);
        s.start();
        
        try {
            //just sleep for now, fix shotdownhook later!!!
            Thread.sleep(100000);
        } catch (InterruptedException ex) {
            System.out.println("wwwwwwwwwww");
        }
    }
    
}
