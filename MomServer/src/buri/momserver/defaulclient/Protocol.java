package buri.momserver.defaulclient;

import java.util.ArrayList;
import java.util.Collection;

/**
 *
 * @author Bela Bursan
 */
public class Protocol {

    static final String PROTOCOL_VERSION = "1.0.0";
    //
    static final String REQUEST_BLINK = "blink";
    static final String REQUEST_EXIT = "exit";

    /**
     * Response OK
     */
    static final String RESPONSE_200 = "200";
    /**
     * Accepted, but not process not completed
     */
    static final String RESPONSE_202 = "202";

    /**
     * Bad request
     */
    static final String RESPONSE_400 = "400";

    /**
     * Request Timeout
     */
    static final String RESPONSE_408 = "408";

    /**
     * Internal server error
     */
    static final String RESPONSE_500 = "500";

    /**
     * Not implemented
     */
    static final String RESPONSE_501 = "501";

    /**
     * Version not supported
     */
    static final String RESPONSE_505 = "505";
    //
    static final int TLV_TAG_MESSAGE = 1;
    static final int TLV_TAG_VERSION = 2;
    static final int TLV_TAG_REQUEST = 3;
    static final int TLV_TAG_TYPE = 4;
    static final int TLV_TAG_ARGUMENT = 5;
    static final int TLV_TAG_RESPONSE = 6;
    static final int TLV_TAG_COMMENT = 7;

    static boolean isValidMessage(TLV tlv, String version) {
        if (tlv.getTag() == TLV_TAG_MESSAGE) {
            TLV t = tlv.findTag(TLV_TAG_VERSION);
            if (t.getValueAsString().equalsIgnoreCase(PROTOCOL_VERSION)) {
                return true;
            }
        }
        return false;
    }

    /**
     *
     * @param message
     * @return
     */
    static String getCommand(TLV message) {
        TLV request = message.findTag(TLV_TAG_REQUEST);
        if (request != null && request.isCdo()) {
            TLV type = request.findTag(TLV_TAG_TYPE);
            if (type != null && !type.isCdo()) {
                return type.getValueAsString();
            }
        }
        return null;
    }

    static Collection<String> getArguments(TLV message) {
        ArrayList<String> list = new ArrayList<>();
        //get the first child
        TLV child = message.findTag(TLV_TAG_REQUEST).getChild();

        while (child != null) {
            if (child.getTag() == TLV_TAG_ARGUMENT) {
                list.add(child.getValueAsString());
            }

            //get next child
            child = child.getNext();
        }

        return list;
    }

    static TLV createResponse(String responseCode, String comment) {
        TLV response = new TLV(TLV_TAG_MESSAGE, TLV.CDO, TLV.CLASS_PRIVATE, null);
        response.addChild(new TLV(TLV_TAG_VERSION, TLV.PDO, TLV.CLASS_PRIVATE, PROTOCOL_VERSION.getBytes()));
        response.addChild(new TLV(TLV_TAG_RESPONSE, TLV.PDO, TLV.CLASS_PRIVATE, responseCode.getBytes()));
        if (comment != null && !comment.equals("")) {
            response.addChild(new TLV(TLV_TAG_COMMENT, TLV.PDO, TLV.CLASS_PRIVATE, comment.getBytes()));
        }

        return response;
    }
}
