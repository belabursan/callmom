package com.hiq.handset.callmom;

import android.app.Activity;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;

/**
 * Created by Patrik on 2015-02-20.
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
        CharSequence text = "Password sent";
        Toast toast = Toast.makeText(context, text.toString(), duration);
        toast.show();
        new SendPasswordTask().execute(text.toString());
    }

    private class SendPasswordTask extends AsyncTask {
        @Override
        protected Object doInBackground(Object[] params) {
            Socket socket = null;
            DataOutputStream dataOutputStream = null;

            try {
                socket = new Socket("192.168.1.100", 2015);
                dataOutputStream = new DataOutputStream(socket.getOutputStream());
                dataOutputStream.writeUTF("Hello");  // The text to send.
            }catch(UnknownHostException e){
                e.printStackTrace();
            }catch(IOException e) {
                e.printStackTrace();
            }finally{
                if(socket != null){
                    try{
                        socket.close();
                    }catch(IOException e){
                        e.printStackTrace();
                    }
                }
                if(dataOutputStream != null){
                    try{
                        dataOutputStream.close();
                    }catch(IOException e){
                        e.printStackTrace();
                    }
                }
            }

            return ((String)params[0]).toString();
        }

        protected void onPostExecute(Object result) {
            /*Context context = getApplicationContext();
            int duration = Toast.LENGTH_SHORT;

            Toast toast = Toast.makeText(context, "dasd", duration);
            toast.show();*/
        }
}
