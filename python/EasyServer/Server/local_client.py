# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-03-17
#
import logging
import threading
import sys
from protocol import Protocol


class LocalClient(threading.Thread):
    """
    Local client, holds a connected socket and runs a protocol to communicate with the remote client
    """

    def __init__(self, connection, working_queue, cleaner_queue, parameters):
        """
        Constructor
        :param connection: socket with a connected client
        :param cleaner_queue: queue to move this client to when finished
        :param parameters: Parameters object, parameters used by the client
        """
        threading.Thread.__init__(self)
        logging.info("LocalClient:init(): creating new client")
        self._name = "not started"
        self._connection = connection
        self._working_queue = working_queue
        self._cleaner_queue = cleaner_queue
        self._params = parameters
        self._end_lock = threading.Lock()

    def run(self):
        """
        Entry point for the client
        Function executed by a thread
        :return:
        """
        try:
            logging.debug("LocalClient:run(): running client")
            protocol = Protocol(self._connection, self._params)
            protocol.execute()
        except:
            logging.error("LocalClient:run(): exception in client: " + str(sys.exc_info()[0]))
            pass
        finally:
            self.close()
            self.end()

    def end(self):
        """
        Ends the client, removes this thread from the working queue and adds it to the cleaner queue.
        Then the cleaner process in the Server will end it if not already done
        """
        try:
            logging.debug("LocalClient:end(): ending client")
            self._working_queue.remove(self)
            self._cleaner_queue.put(self, True)
        except:
            logging.debug("LocalClient:end(): exception when ending client" + sys.exc_info()[0])
            pass

    def close(self):
        """
        Closes the client socket
        """
        try:
            self._end_lock.acquire()
            logging.debug("LocalClient:close(): closing client")

            if self._connection:
                self._connection.close()
                self._connection = None
        except:
            logging.debug("LocalClient:close(): exception when closing client" + sys.exc_info()[0])
            pass
        finally:
            self._end_lock.release()

