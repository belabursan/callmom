package buri.momserver;

/**
 *
 * @author belabursan
 */
class Server extends Thread implements Runnable {
    boolean alive;
    
    
    
    @Override
    public void run(){
        alive = true;
        while(alive){
            System.out.println("ruuun");
            alive = false;
        }
    }
}
