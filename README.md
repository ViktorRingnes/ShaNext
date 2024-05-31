
# ShaNext

ShaNext is a comprehensive security hashing library for .NET, designed to handle a variety of hashing needs such as hashing strings, comparing hashes, hashing files, and comparing file hashes. This library supports asynchronous operations to ensure efficient performance in modern applications.

## Features

- **String Hashing:** Hash any string using the custom SHA algorithm.
- **Salted Hashing:** Generate salted hashes to enhance security.
- **File Hashing:** Hash the contents of any file.
- **Custom Iterations:** Perform hash operations with custom iterations.
- **Secure Comparison:** Time-safe comparisons of hashes to mitigate timing attacks.
- **Asynchronous Operations:** All major functions support async versions for non-blocking performance.
- **Scrypt Hashing:** Hash any string using the Scrypt algorithm.
- **Argon2 Hashing:** Hash any string using the Argon2 algorithm.
- **Base64 Encoding/Decoding:** Encode and decode data using Base64.
- **Hex Encoding/Decoding:** Encode and decode data using Hexadecimal.
- **HMAC:** Generate and verify HMACs.
- **PBKDF2:** Generate and verify keys using PBKDF2.

## Installation

To install ShaNext, use the NuGet Package Manager in Visual Studio or run the following command in your terminal:
```bash
dotnet add package ShaNext
```
Or download it from [Nuget.](https://www.nuget.org/packages/ShaNext/)

## Configuration

ShaNext uses a configuration file (hash_config.json) to specify the default hashing algorithm. By default, SHA-256 is used, which is an industry-standard algorithm.

### Default Configuration (hash_config.json)
```json
{
  "default_algorithm": "SHA_256"
}
```
!! If the hash_config.json is not present / automatically created in the debug folder, create one with the contents above.

You can change the default hashing algorithm by editing the hash_config.json file in your project's output directory (e.g., bin/Debug/net8.0/). Supported algorithms include:

- SHA_1
- SHA_224
- SHA_256
- SHA_384
- SHA_512
- SHA_512_224
- SHA_512_256
- SHA_3
- SHAKE128
- MD5
- WHIRLPOOL
- RIPEMD_160
- SCRYPT
- ARGON2

## Usage

### Hashing Strings

```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        string input = "testInput";
        
        // Synchronous Hashing
        string hash = ShaNextHashing.Hash(input);
        Console.WriteLine($"Hash: {hash}");
        
        // Asynchronous Hashing
        string asyncHash = await ShaNextHashing.HashAsync(input);
        Console.WriteLine($"Async Hash: {asyncHash}");
    }
}
```
### Hashing with Salt
```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        string input = "testInput";
        string salt = ShaNextSalt.NewSalt();
        
        // Synchronous Salted Hashing
        string saltedHash = ShaNextHashing.HashWithSalt(input, salt);
        Console.WriteLine($"Salted Hash: {saltedHash}");
        
        // Asynchronous Salted Hashing
        string asyncSaltedHash = await ShaNextHashing.HashWithSaltAsync(input, salt);
        Console.WriteLine($"Async Salted Hash: {asyncSaltedHash}");
    }
}
```

### Generating and Verifying Salted Hashes
```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        string input = "testInput";
        
        // Synchronous Generation and Verification
        string saltedHashStored = ShaNextHashing.GenerateSaltedHash(input);
        bool isValid = ShaNextCompare.VerifySaltedHash(input, saltedHashStored);
        Console.WriteLine($"Is Valid: {isValid}");
        
        // Asynchronous Generation and Verification
        string asyncSaltedHashStored = await ShaNextHashing.GenerateSaltedHashAsync(input);
        bool asyncIsValid = await ShaNextCompare.VerifySaltedHashAsync(input, asyncSaltedHashStored);
        Console.WriteLine($"Async Is Valid: {asyncIsValid}");
    }
}
```

### File Hashing
```csharp
using ShaNext.ShaNext;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "C:\path\to\your\file.txt";
        
        // Synchronous File Hashing
        string fileHash = ShaNextHashing.HashFile(filePath);
        Console.WriteLine($"File Hash: {fileHash}");
        
        // Asynchronous File Hashing
        string asyncFileHash = await ShaNextHashing.HashFileAsync(filePath);
        Console.WriteLine($"Async File Hash: {asyncFileHash}");
    }
}
```
### Time-Safe Comparison
```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static void Main(string[] args)
    {
        string hash1 = "abcdef";
        string hash2 = "abcdef";
        
        bool isEqual = ShaNextUtilities.TimeSafeCompare(hash1, hash2);
        Console.WriteLine($"Hashes are equal: {isEqual}");
    }
}
```

### Base64 Encoding/Decoding
```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static void Main(string[] args)
    {
        string data = "testInput";
        
        // Base64 Encoding
        string base64Encoded = ShaNextBase64.EncodeString(data);
        Console.WriteLine($"Base64 Encoded: {base64Encoded}");
        
        // Base64 Decoding
        string base64Decoded = ShaNextBase64.DecodeToString(base64Encoded);
        Console.WriteLine($"Base64 Decoded: {base64Decoded}");
    }
}
```

### Hex Encoding/Decoding
```csharp
using ShaNext.ShaNext;
using System;

class Program
{
    static void Main(string[] args)
    {
        string data = "testInput";
        
        // Hex Encoding
        string hexEncoded = ShaNextHex.EncodeString(data);
        Console.WriteLine($"Hex Encoded: {hexEncoded}");
        
        // Hex Decoding
        string hexDecoded = ShaNextHex.DecodeToString(hexEncoded);
        Console.WriteLine($"Hex Decoded: {hexDecoded}");
    }
}
```

### HMAC
```csharp
using ShaNext.ShaNext;
using System;
using System.Security.Cryptography;

class Program
{
    static async Task Main(string[] args)
    {
        string key = "secretKey";
        string data = "testInput";
        
        // Generate HMAC
        string hmac = ShaNextHMAC.GenerateHMAC(key, data, HashAlgorithmName.SHA256);
        Console.WriteLine($"HMAC: {hmac}");
        
        // Verify HMAC
        bool isValid = ShaNextHMAC.VerifyHMAC(key, data, hmac, HashAlgorithmName.SHA256);
        Console.WriteLine($"HMAC is valid: {isValid}");
        
        // Asynchronous Generate HMAC
        string asyncHmac = await ShaNextHMAC.GenerateHMACAsync(key, data, HashAlgorithmName.SHA256);
        Console.WriteLine($"Async HMAC: {asyncHmac}");
        
        // Asynchronous Verify HMAC
        bool asyncIsValid = await ShaNextHMAC.VerifyHMACAsync(key, data, hmac, HashAlgorithmName.SHA256);
        Console.WriteLine($"Async HMAC is valid: {asyncIsValid}");
    }
}
```

### PBKDF2
```csharp
using ShaNext.ShaNext;
using System;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string password = "password";
        byte[] salt = ShaNextSalt.NewSalt();
        int iterations = 10000;
        int keyLength = 32;
        
        // Generate PBKDF2 Key
        byte[] key = ShaNextPBKDF2.GenerateKey(password, salt, iterations, keyLength);
        string keyBase64 = Convert.ToBase64String(key);
        Console.WriteLine($"PBKDF2 Key: {keyBase64}");
        
        // Verify PBKDF2 Key
        bool isValid = ShaNextPBKDF2.VerifyKey(password, salt, iterations, key);
        Console.WriteLine($"PBKDF2 Key is valid: {isValid}");
        
        // Asynchronous Generate PBKDF2 Key
        byte[] asyncKey = await ShaNextPBKDF2.GenerateKeyAsync(password, salt, iterations, keyLength);
        string asyncKeyBase64 = Convert.ToBase64String(asyncKey);
        Console.WriteLine($"Async PBKDF2 Key: {asyncKeyBase64}");
        
        // Asynchronous Verify PBKDF2 Key
        bool asyncIsValid = await ShaNextPBKDF2.VerifyKeyAsync(password, salt, iterations, key);
        Console.WriteLine($"Async PBKDF2 Key is valid: {asyncIsValid}");
    }
}
```


# API Reference

## ShaNextHashing Class

### Methods

#### Synchronous Methods

- **Hash(string input):**  
  Computes the hash of the given input string.

- **HashWithSalt(string input, string salt):**  
  Computes the hash of the given input string concatenated with a salt.

- **GenerateSaltedHash(string input):**  
  Generates a new salt and computes the hash of the input string concatenated with the salt. Returns the hash and the salt.

- **HashWithCustomIterations(string input, string salt, int iterations):**  
  Computes the hash of the input string concatenated with the salt, iteratively for a specified number of iterations.

- **HashFile(string filePath):**  
  Computes the hash of the contents of the specified file.

- **HashStream(Stream stream):**  
  Computes the hash of the contents of the provided stream.

#### Asynchronous Methods

- **HashAsync(string input):**  
  Asynchronously computes the hash of the given input string.

- **HashWithSaltAsync(string input, string salt):**  
  Asynchronously computes the hash of the given input string concatenated with a salt.

- **GenerateSaltedHashAsync(string input):**  
  Asynchronously generates a new salt and computes the hash of the input string concatenated with the salt. Returns the hash and the salt.

- **HashWithCustomIterationsAsync(string input, string salt, int iterations):**  
  Asynchronously computes the hash of the input string concatenated with the salt, iteratively for a specified number of iterations.

- **HashFileAsync(string filePath):**  
  Asynchronously computes the hash of the contents of the specified file.

- **HashStreamAsync(Stream stream):**  
  Asynchronously computes the hash of the contents of the provided stream.

## ShaNextCompare Class

### Methods

#### Synchronous Methods

- **Compare(string input, string hash):**  
  Compares the hash of the input string with the provided hash.

- **CompareWithSalt(string input, string salt, string hash):**  
  Compares the hash of the input string concatenated with the salt with the provided hash.

- **VerifySaltedHash(string input, string storedHash):**  
  Verifies the input string against a stored salted hash.

- **VerifyFileHash(string filePath, string expectedHash):**  
  Verifies the hash of the specified file against the expected hash.

#### Asynchronous Methods

- **CompareAsync(string input, string hash):**  
  Asynchronously compares the hash of the input string with the provided hash.

- **CompareWithSaltAsync(string input, string salt, string hash):**  
  Asynchronously compares the hash of the input string concatenated with the salt with the provided hash.

- **VerifySaltedHashAsync(string input, string storedHash):**  
  Asynchronously verifies the input string against a stored salted hash.

- **VerifyFileHashAsync(string filePath, string expectedHash):**  
  Asynchronously verifies the hash of the specified file against the expected hash.

## ShaNextSalt Class

### Methods

- **NewSalt(string providedSalt = null):**  
  Generates a new salt. If a salt is provided, it returns the provided salt.

- **NewFixedLengthSalt(int length):**  
  Generates a new salt of the specified length.

## ShaNextUtilities Class

### Methods

- **TimeSafeCompare(string a, string b):**  
  Compares two strings in a time-safe manner to mitigate timing attacks.

## ShaNextPBKDF2 Class

### Methods

#### Synchronous Methods

- **GenerateKey(string password, byte[] salt, int iterations, int keyLength):**  
  Generates a PBKDF2 key for the given password and salt.

- **VerifyKey(string password, byte[] salt, int iterations, byte[] expectedKey):**  
  Verifies the PBKDF2 key against the expected key.

#### Asynchronous Methods

- **GenerateKeyAsync(string password, byte[] salt, int iterations, int keyLength):**  
  Asynchronously generates a PBKDF2 key for the given password and salt.

- **VerifyKeyAsync(string password, byte[] salt, int iterations, byte[] expectedKey):**  
  Asynchronously verifies the PBKDF2 key against the expected key.

## ShaNextHMAC Class

### Methods

#### Synchronous Methods

- **GenerateHMAC(string key, string data, HashAlgorithmName hashAlgorithmName):**  
  Generates an HMAC for the given key and data using the specified hash algorithm.

- **VerifyHMAC(string key, string data, string hmac, HashAlgorithmName hashAlgorithmName):**  
  Verifies the HMAC against the expected HMAC.

#### Asynchronous Methods

- **GenerateHMACAsync(string key, string data, HashAlgorithmName hashAlgorithmName):**  
  Asynchronously generates an HMAC for the given key and data using the specified hash algorithm.

- **VerifyHMACAsync(string key, string data, string hmac, HashAlgorithmName hashAlgorithmName):**  
  Asynchronously verifies the HMAC against the expected HMAC.

## ShaNextBase64 Class

### Methods

- **Encode(byte[] data):**  
  Encodes the given data to a Base64 string.

- **Decode(string base64String):**  
  Decodes the given Base64 string to data.

- **EncodeString(string data):**  
  Encodes the given string to a Base64 string.

- **DecodeToString(string base64String):**  
  Decodes the given Base64 string to a string.

## ShaNextHex Class

### Methods

- **Encode(byte[] data):**  
  Encodes the given data to a Hexadecimal string.

- **Decode(string hex):**  
  Decodes the given Hexadecimal string to data.

- **EncodeString(string data):**  
  Encodes the given string to a Hexadecimal string.

- **DecodeToString(string hex):**  
  Decodes the given Hexadecimal string to a string.

# License

ShaNext is licensed under the MIT License. See the LICENSE file for more information.
