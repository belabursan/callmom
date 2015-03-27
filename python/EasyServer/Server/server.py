# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-02-28
#

import logging
import threading
import atexit
import socket
import time
try:
    import Queue as queue
except:
    import queue

from local_client import LocalClient
from multiprocessing import Process


class Server(Process):
    """
    Server class, contains the main thread of the server
    """

    def __init__(self, main):
        """
        Constructor
        :param main: reference to the Main class
        """
        super(Server, self).__init__(group=None, name="ServerProcess")
        self.name = "ServerProcess"
        self._main = main
        self._parameters = main.parameters
        self._lock = threading.Lock()
        self._server_socket = None
        self._isAlive = False
        self._working_list = []
        self._cleaner_queue = queue.Queue(101)
        self._cleaner = None
        atexit.register(main.shut_down_hook, None, None)

    def run(self):
        """
        Entry point for the server
        Runs the main server thread, creates a cleaner object, holds the server socket
        and creates local clients to communicate with remote clients
        """
        logging.debug("Server:run(): running server on port " + str(self._parameters.port))
        self._isAlive = True
        self._cleaner = Cleaner(self, self._cleaner_queue, self._working_list)
        self._cleaner.start()
        connection = None

        try:
            server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server_socket.bind(('localhost', self._parameters.port))
            logging.info("Server:run(): Socket bind complete")
            server_socket.listen(self._parameters.noof_sockets)
            logging.info("Server:run(): Socket now listening")

            while self._isAlive:
                connection, address = server_socket.accept()
                connection.settimeout(self._parameters.timeout_seconds)
                logging.debug("Server:run(): connected with " + str(address[0]) + ":" + str(address[1]))

                client = LocalClient(connection, self._working_list, self._cleaner_queue, self._parameters)

                # if working threads are greater then allowed wait a second
                while len(self._working_list) >= self._parameters.noof_threads:
                    time.sleep(1)
                self._working_list.append(client)
                client.start()
                # logging.info("worksize 0: " + str(len(self._working_list)))
                # logging.info("clena 0: " + str(self._cleaner_queue.qsize()))

        except KeyboardInterrupt:
            logging.debug("Server:run(): Got KeyboardInterrupt in Server")
            # pass this to main and end everything
            pass
        except Exception as ex:
            logging.exception("Server:run(): Exception: " + str(ex))
            pass
        finally:
            if connection:
                connection.close()
            if self._server_socket:
                self._server_socket.close()

    def stop_server(self):
        """
        Stops the server, closes the cleaner
        :return:
        """
        self._isAlive = False
        self._lock.acquire()
        logging.debug("Server:stop_server(): ending server")
        if self._server_socket:
            self._server_socket.close()

        self._lock.release()
        if self._cleaner:
            self._cleaner.close()
            self._cleaner.join()
        while len(self._working_list) > 0:
            try:
                self._cleaner_queue.put(self._working_list.pop(0))
            except:
                pass
        while self._cleaner_queue.qsize() > 0:
            try:
                c = self._cleaner_queue.get()
                c.close()
                c.join()
            except:
                pass


class Cleaner(Process):
    """
    Cleaner, closes all local clients in the cleaner list
    """
    def __init__(self, server, cleaner_queue, working_queue):
        """
        Constructor for the cleaner
        """
        super(Cleaner, self).__init__(group=None, name="CleanerProcess")
        self.daemon = True
        self._server = server
        self._cleaner_queue = cleaner_queue
        self._working_queue = working_queue
        self._alive = False

    def run(self):
        """
        Entry point, runs the cleaner
        """
        logging.debug("Cleaner:run(): cleaner started")
        self._alive = True
        while self._alive:
            try:
                client = self._cleaner_queue.get(True, 1200)
                message = client.close()
                client.join()
                logging.debug("Cleaner:run(): " + str(message))
            except queue.Empty:
                # logging.debug("empty exception in cleaner")
                # logging.debug("workqueuesize 1: " + str(len(self._working_queue)))
                # logging.debug("cleanerqueuesize 1: " + str(self._cleaner_queue.qsize()))
                pass
            except KeyboardInterrupt:
                logging.info("Cleaner:run(): KeyboardInterrupt exception in cleaner")
                break

    def close(self):
        """
        Closes the cleaner
        """
        logging.debug("Cleaner:close(): closing the cleaner")
        self._alive = False
        self.terminate()