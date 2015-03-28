# EasyServer to listen and send answer to incoming socket call
# Bela Bursan<burszan@gmail.com> - 2015-02-28
#
# Sources:
# https://launchkey.com/docs/api/encryption
# http://sentdex.com/sentiment-analysisbig-data-and-python-tutorials-algorithmic-trading/encryption-and-decryption-in-python-code-example-with-explanation/

import os
import logging
import hashlib
from Crypto.PublicKey import RSA
from Crypto.Cipher import PKCS1_OAEP, AES
from base64 import b64decode, b64encode


class BCrypt(object):
    """
    Holds some functions to encrypt/decrypt strings
    """
    def __init__(self, params, padding, block_size):
        """
        Constructor
        :param params: - parameters from the applications ini file
        :param padding: padding used for AES crypto
        :param block_size: block size used by AES
        :return:
        """
        self._padding = padding
        self._block_size = block_size
        self._params = params

    def create_random(self, length=None):
        """
        Creates a random
        :param length: length of the random
        :return: random
        """
        if length is None:
            length = self._block_size
        return os.urandom(length)

    def generate_hash(self, data):
        """
        Generates a sha256 hash of a text string
        :param data: data to get the hash for
        :return: the hash of the data in argument
        """
        return hashlib.sha256(data).hexdigest()

    def create_RSA_key(self, data):
        """
        Creates an rsa key from data received from a socket or file
        :param data:
        :return: an RSA public key
        """
        return PKCS1_OAEP.new(RSA.importKey(data))

    def encrypt_RSA(self, rsa_key, data):
        """
        Encrypts a string with the public key from the parameter
        :param: rsa_key Public key
        :param: message String to be encrypted
        :return base64 encoded encrypted string
        """
        logging.debug("BCrypt:encrypt_RSA(): encrypting data")
        return rsa_key.encrypt(data).encode('base64')

    def decrypt_RSA(self, rsa_key, package):
        """"
        param: public_key_loc rsa key
        param: package String to be decrypted
        return decrypted string
        """
        logging.debug("BCrypt:decrypt_RSA(): decrypting data")
        return rsa_key.decrypt(b64decode(package))

    def decrypt_AES(self, aes_key, data):
        """
        Decrypts a string encrypted with AES
        :param data: string to decrypt
        :param aes_key: key to decrypt with
        :return: the decrypted string
        """
        logging.debug("BCrypt:decrypt_AES(): decrypting")
        decoder = lambda cipher, in_data: cipher.decrypt(b64decode(in_data)).rstrip(self._padding)
        return decoder(AES.new(aes_key), data)

    def encrypt_AES(self, aes_key, data):
        """
        Encrypts a string with AES
        :param aes_key: key to encrypt with
        :param data: data to encrypt
        :return: the encrypted string
        """
        logging.debug("BCrypt:encrypt_AES(): encrypting")
        pad = lambda x: x + (self._block_size - len(x) % self._block_size) * self._padding
        encoder = lambda cipher, in_data: b64encode(cipher.encrypt(pad(in_data)))

        return encoder(AES.new(aes_key), data)
