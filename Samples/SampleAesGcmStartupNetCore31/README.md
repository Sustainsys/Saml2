# Adding AES-GCM support for SustainSys.Saml2

To add AES-GCM support for decrypting SAML messages, a SymmetricAlgorithm implementation must be provided and registered with `System.Security.Cryptography.CryptoConfig.AddAlgorithm()` 

This sample project demonstrates how such implementation could be done in .Net Core 3.1.

See `AesGcmAlgorithm.cs` for the SymmetricAlgorithm implementation. It is a wrapper for the actual AES-GCM implementation. The used AES-GCM implementation is available in .Net Core 3.1 and .Net Standard 2.1. For .Net Core 2.x and .Net Framework, the some other implementation must be used (for examle, BouncyCastle).
