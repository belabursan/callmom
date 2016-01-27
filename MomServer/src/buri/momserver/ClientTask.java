package buri.momserver;

import java.io.InputStream;
import java.net.Socket;

/**
 *
 * @author belabursan
 */
public class ClientTask extends IClientTask {

    public ClientTask(Socket socket) {
        super(socket);
    }

    @Override
    public void run() {
        System.out.println("new client: " + Thread.currentThread().getName());
        try{
            byte[] buffer = new byte[8196];
            while(alive){
                InputStream is = socket.getInputStream();
                is.read(buffer);
                String s = new String(buffer);
                String trim = s.trim();
                if(trim.equals("xxx")){
                    System.out.println("GOT xxxxxx");
                    alive = false;
                }
                else{
                    System.out.println("---got " + trim);
                }
            }
            close();
        }catch(Exception ex){
            
        } finally{
            System.out.println("closing client " + Thread.currentThread().getName());
        }
    }
}
