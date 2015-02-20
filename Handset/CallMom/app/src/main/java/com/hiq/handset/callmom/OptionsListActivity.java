package com.hiq.handset.callmom;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

/**
 * Created by Patrik on 2015-02-09.
 */
public class OptionsListActivity extends ListActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.options_list_activity);

        String[] options = {"Register","Unregister"};
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, options);
        setListAdapter(adapter);
    }

    @Override
    protected void onStart() {
        super.onStart();
        // The activity is about to become visible.
    }

    @Override
    protected void onResume() {
        super.onResume();
        // The activity has become visible (it is now "resumed").
    }

    @Override
    protected void onPause() {
        super.onPause();
        // Another activity is taking focus (this activity is about to be "paused").
    }

    @Override
    protected void onStop() {
        super.onStop();
        // The activity is no longer visible (it is now "stopped")
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        // The activity is about to be destroyed.
    }

    protected void onListItemClick(ListView parent, View v, int position, long id){
        String item = (String) getListAdapter().getItem(position);
        Toast.makeText(this, item + " selected", Toast.LENGTH_SHORT).show();

        if(0 == position) {
            Intent startRegisterActivityIntent = new Intent(this, RegisterActivity.class);
            if (startRegisterActivityIntent.resolveActivity(getPackageManager()) != null)
                startActivity(startRegisterActivityIntent);
        }else if(1 == position){
            Intent startUnregisterActivityIntent = new Intent(this, UnregisterActivity.class);
            if (startUnregisterActivityIntent.resolveActivity(getPackageManager()) != null)
                startActivity(startUnregisterActivityIntent);
        }

    }
}

