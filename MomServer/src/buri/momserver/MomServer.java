package buri.momserver;

import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author belabursan
 */
public class MomServer {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        Server s = new Server();
        s.start();
        
        try {
            //just sleep for now, fix shotdownhook later!!!
            Thread.sleep(100000);
        } catch (InterruptedException ex) {
            Logger.getLogger(MomServer.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
}
