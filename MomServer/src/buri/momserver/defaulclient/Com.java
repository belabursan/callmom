package buri.momserver.defaulclient;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.util.LinkedList;

/**
 * Com module for the client
 *
 * @author Bela Bursan
 */
final class Com {

    private static final int BUFFER_SIZE = 8192;
    //
    private final Socket socket;
    private final LinkedList<TLV> tlvList;
    private final LinkedList<String> logger;
    private final boolean debugging;
    private int last, byteBufLen;
    private final BufferedInputStream streamIn;
    private final BufferedOutputStream streamOut;
    private final byte[] byteBuffer;
    private byte[] returned;

    Com(Socket socket, boolean debug, LinkedList<String> logger) throws IOException {
        this.socket = socket;
        this.debugging = debug;
        this.logger = logger;
        this.tlvList = new LinkedList<>();
        this.byteBuffer = new byte[BUFFER_SIZE];
        streamIn = new BufferedInputStream(socket.getInputStream(), BUFFER_SIZE);
        streamOut = new BufferedOutputStream(socket.getOutputStream(), BUFFER_SIZE);
        this.byteBufLen = 0;
        this.last = 0;
        socket.setTcpNoDelay(true);
    }

    /**
     * Log
     *
     * @param text text to add to the logger
     */
    private void log(String text) {
        logger.add(text);
    }

    /**
     * Log, only if debug flag is set
     *
     * @param text text to add to the logger
     */
    private void debug(String text) {
        if (this.debugging) {
            log(text);
        }
    }

    /**
     * Reads the network and returns a TLV object
     *
     * @param timeout time in milliseconds to wait to receive data
     * @return TLV object or throws exception
     * @throws SocketException in case of no data was received and timeout
     * occurs
     * @throws IOException in case of network problem
     * @throws IllegalArgumentException in case of corrupt data is received
     */
    TLV read(int timeout) throws SocketTimeoutException, IOException, IllegalArgumentException {
        //setting timeout
        this.socket.setSoTimeout(timeout);
        if (this.tlvList.size() > 0) {
            debug("returning message from tlv buffer");
            return tlvList.removeFirst();
        }
        TLV tlv = new TLV();
        last = 0;
        while (true) {
            try {
                last = streamIn.read(byteBuffer, byteBufLen, byteBuffer.length - byteBufLen);
                if (last < 0) {
                    throw new IOException("end of stream reached");
                }
                byteBufLen += last;

                returned = tlv.createFromByteArray(byteBuffer, 0, byteBufLen);
                break;
            } catch (IllegalArgumentException ig) {
                if (byteBufLen > 1024) {
                    debug("ERROR: illegal argument from tlv(corrupt tlv?): " + ig.getMessage());
                    throw ig;
                }
                debug("got illegal arg in first while, not complete or corrupt? reading more bytes...");
            }
        }
        byteBufLen = 0;
        try {
            while (returned != null && returned.length > 0) {
                TLV tt = new TLV();
                returned = tt.createFromByteArray(returned, 0, returned.length);
                this.tlvList.addLast(tt);
            }
        } catch (IllegalArgumentException iae) {
            byteBufLen = returned.length;
            System.arraycopy(returned, 0, byteBuffer, 0, byteBufLen);
            debug("could not create tlv, got illegal arg exception: " + iae.getMessage());
            debug("copying the rest to byteArray, len: " + byteBufLen);
        }

        return tlv;
    }
    
    /**
     * Sends a TLV object as bytes to the network
     *
     * @param response TLV object to send
     * @throws IOException in case of network error
     */
    void send(TLV response) throws IOException {
        if (response != null) {
            byte[] bytes = response.convertToByteArray();
            streamOut.write(bytes, 0, bytes.length);
            streamOut.flush();
        }
    }

    /**
     * Closes the communication module
     */
    void close() {
        if (streamOut != null) {
            try {
                streamOut.flush();
                streamOut.close();
            } catch (IOException ex) {
                //ignore
            }
        }
        if (streamIn != null) {
            try {
                streamIn.close();
            } catch (IOException ex) {
                //ignore
            }
        }
        tlvList.clear();
    }

}
