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
import binascii
import StringIO
from base64 import b64decode, b64encode


class BCrypt(object):
    """
    Holds some functions to encrypt/decrypt strings
    """
    def __init__(self, params, block_size=16):
        """
        Constructor
        :param params: - parameters from the applications ini file
        :param block_size: block size used by AES
        :return:
        """
        self._block_size = block_size
        self._params = params
        self._aes_iv = '\x42\x75\x72\x73\x61\x6e\x42\x65\x6c\x61\x4c\x61\x73\x7a\x6c\x6f'
        self._aes_mode = AES.MODE_CBC

    def create_random(self, length=None):
        """
        Creates a random
        :param length: length of the random
        :return: random
        """
        if length is None:
            length = self._block_size
        return os.urandom(length)

    def get_hash_as_string(self, data):
        """
        Generates a sha256 hash of a text string
        :param data: data to get the hash for
        :return: the hash of the data in argument as string
        """
        return hashlib.sha256(data).hexdigest()

    def get_hash_as_bytes(self, data):
        """
        Generates a sha256 hash of a text string
        :param data: data to get the hash for
        :return: the hash of the data in argument as bytes
        """
        return hashlib.sha256(data).digest()

    def encrypt_RSA(self, rsa_key, data):
        """
        Encrypts a string with the public key from the parameter
        :param: rsa_key Public key
        :param: message String to be encrypted
        :return base64 encoded encrypted string
        """
        logging.debug("BCrypt:encrypt_RSA(): encrypting data")
        return rsa_key.encrypt(data).encode('base64')

    def decrypt_RSA(self, rsa_key_path, data):
        """"
        param: public_key_loc rsa key
        param: package String to be decrypted
        return decrypted string
        """
        logging.debug("BCrypt:decrypt_RSA(): decrypting data")

        encrypted = b64decode(data)

        rsa_key = RSA.importKey(open(rsa_key_path, "r").read())
        oaep_key = PKCS1_OAEP.new(rsa_key)

        res = oaep_key.decrypt(encrypted)

        """
        print("datalen:" + str(len(data)))
        print("reslen" + str(len(res)))
        print("zlen: " + str(len(z)))
        print("XXX: " + z.encode("hex"))
        """

        return res

    def decrypt_AES(self, key, data):
        """
        Decrypts a string encrypted with AES
        :param data: string to decrypt
        :param key: key to decrypt with, must be a hex-string with proper length(16-24-32)
        :return: the decrypted string
        """
        logging.debug("BCrypt:decrypt_AES(): decrypting")

        decrypted = self.make_aes_key(key).decrypt(b64decode(str(data)))
        return decrypted[:(len(decrypted) - int(binascii.hexlify(decrypted[-1]), 16))]

    def encrypt_AES(self, clear_key, data):
        """
        Encrypts a string with AES
        :param clear_key: key to encrypt with
        :param data: data to encrypt
        :return: the encrypted string
        """
        logging.debug("BCrypt:encrypt_AES(): encrypting")

        output = StringIO.StringIO()
        val = self._block_size - (len(data) % self._block_size)
        for _ in xrange(val):
            output.write('%02x' % val)
        data += binascii.unhexlify(output.getvalue())

        return b64encode(self.make_aes_key(clear_key).encrypt(data))

    def make_aes_key(self, aes_key):
        """
        Creates an aes key
        :param aes_key: clear text string to transform to an aes key
        :return: aes key
        """
        key = AES.new(aes_key.decode("hex"), self._aes_mode, self._aes_iv)
        return key
