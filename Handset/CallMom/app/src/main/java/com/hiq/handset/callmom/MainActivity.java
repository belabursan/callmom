package com.hiq.handset.callmom;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;


public class MainActivity extends ActionBarActivity{

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu items for use in the action bar
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.main_activity_actions, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle presses on the action bar items
        switch (item.getItemId()) {
            case R.id.options:
                openOptions();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    /** Called when the user touches the button */
    private void openOptions() {
        // Do something in response to button click
        Context context = getApplicationContext();
        CharSequence text = "Options pressed!";
        int duration = Toast.LENGTH_SHORT;

        Toast toast = Toast.makeText(context, text, duration);
        toast.show();

        Intent optionsIntent = new Intent(this,OptionsListActivity.class);
        if(optionsIntent.resolveActivity(getPackageManager()) != null)
            startActivity(optionsIntent);
    }

    /** Called when the user touches the button */
    public void notify(View view) {
        // Do something in response to button click
        Context context = getApplicationContext();
        int duration = Toast.LENGTH_SHORT;
        CharSequence text = "Lights on!";
        Toast toast = Toast.makeText(context, text.toString(), duration);
        toast.show();
        new SocketSendTask().execute(text.toString());
    }

    private class SocketSendTask extends AsyncTask {
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
            }catch(IOException e){
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
}
