# ShaNext

ShaNext is a comprehensive security hashing library for .NET, designed to handle a variety of hashing needs such as hashing strings, comparing hashes, hashing files, and comparing file hashes. This library supports asynchronous operations to ensure efficient performance in modern applications.

## Features

- **String Hashing:** Hash any string using the custom SHA algorithm.
- **Salted Hashing:** Generate salted hashes to enhance security.
- **File Hashing:** Hash the contents of any file.
- **Custom Iterations:** Perform hash operations with custom iterations.
- **Secure Comparison:** Time-safe comparisons of hashes to mitigate timing attacks.
- **Asynchronous Operations:** All major functions support async versions for non-blocking performance.

## Installation

To install ShaNext, use the NuGet Package Manager in Visual Studio or run the following command in your terminal:
```bash
dotnet add package ShaNext
```

## Configuration

ShaNext uses a configuration file (hash_config.json) to specify the default hashing algorithm. By default, SHA-256 is used, which is an industry-standard algorithm.

### Default Configuration (hash_config.json)
```json
{
  "default_algorithm": "SHA_256"
}
```

You can change the default hashing algorithm by editing the hash_config.json file in your project's output directory (e.g., bin/Debug/net5.0/). Supported algorithms include:

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

## Usage
Below are some examples of how to use the ShaNext library.

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
        string filePath = "C:\\path\\to\\your\\file.txt";
        
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


## Contributing

We welcome contributions! If you'd like to contribute, please fork the repository and use a feature branch. Pull requests are warmly welcome.

- Fork the repository.
- Create a feature branch (git checkout -b feature/your-feature).
- Commit your changes (git commit -am 'Add some feature').
- Push to the branch (git push origin feature/your-feature).
- Create a new Pull Request.

## License

ShaNext is licensed under the MIT License. See the LICENSE file for more information.
