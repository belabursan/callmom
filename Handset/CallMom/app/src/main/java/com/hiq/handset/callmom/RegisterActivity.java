package com.hiq.handset.callmom;

import android.app.Activity;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;

/**
 * Created by Patrik on 2015-02-20.
 *
 * The idea of the class is for a user to be able
 * to register to the service so that the user is
 * allowed to turn on and off the lamp.
 *
 * The user enters a password and sends the
 * password to the raspberry. The raspberry then
 * verifies the password and upon a correct password
 * sends a unique token to the user. The user use the
 * token go gain access to the service after the initial
 * registration.
 * A user only register once for the service and then use
 * the token automatically to gain access to the service.
 *
 * TODO: This class RegisterActivity, needs to be finished by someone.
 *
 */
public class RegisterActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.register_activity);
    }

    /** Called when the user touches the button */
    public void register(View view) {
        // Do something in response to button click
        Context context = getApplicationContext();
        int duration = Toast.LENGTH_SHORT;
        Button button = (Button) view.findViewById(R.id.button_ok);
        CharSequence password = button.getText();
        Toast toast = Toast.makeText(context, "Sending password", duration);
        toast.show();
        new SendPasswordTask().execute(password.toString());
    }

    private class ReceiveTokenTask extends AsyncTask {
        @Override
        protected Object doInBackground(Object[] params) {
            ServerSocket serverSocket = null;
            Socket socket = null;
            DataInputStream dataInputStream = null;

            try {
                serverSocket = new ServerSocket(8888);
            } catch (IOException e) {
                e.printStackTrace();
            }

            while(true) {
                try {
                    socket = serverSocket.accept();
                    dataInputStream = new DataInputStream(socket.getInputStream());
                } catch (IOException e){
                    e.printStackTrace();
                }finally {
                    if(socket != null){
                        try{
                            socket.close();
                        }catch(IOException e){
                            e.printStackTrace();
                        }
                    }

                    if(dataInputStream !=null){
                        try{
                            dataInputStream.close();
                        }catch(IOException e){
                            e.printStackTrace();
                        }
                    }
                }
            }
        }
    }

    private class SendPasswordTask extends AsyncTask {
        @Override
        protected Object doInBackground(Object[] params) {
            Socket socket = null;
            DataOutputStream dataOutputStream = null;

            try {
                socket = new Socket("192.168.1.100", 2015);
                dataOutputStream = new DataOutputStream(socket.getOutputStream());
                dataOutputStream.writeUTF(((String) params[0]).toString());  // The text to send.
            } catch (UnknownHostException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            } finally {
                if (socket != null) {
                    try {
                        socket.close();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
                if (dataOutputStream != null) {
                    try {
                        dataOutputStream.close();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }

            return ((String) params[0]).toString();
        }
    }

        protected void onPostExecute(Object result) {
            /*Context context = getApplicationContext();
            int duration = Toast.LENGTH_SHORT;

            Toast toast = Toast.makeText(context, "dasd", duration);
            toast.show();*/
        }
}
