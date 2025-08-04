using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace game_x.infrastructure.Security.Asymmetric;

public class AsymmetricCryptoService : IAsymmetricCryptoService, IServices
{
    public (string PublicKeyPem, string PrivateKeyPem) GenerateKeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var publicKeyPem = ExportPublicKeyToPem(ecdsa);
        var privateKeyPem = ExportPrivateKeyToPem(ecdsa);

        return (publicKeyPem, privateKeyPem);
    }

    private string ExportPublicKeyToPem(ECDsa ecdsa)
    {
        return ecdsa.ExportSubjectPublicKeyInfoPem();
    }

    private string ExportPrivateKeyToPem(ECDsa ecdsa)
    {
        return ecdsa.ExportPkcs8PrivateKeyPem();
    }

    public string Sign(string privateKeyPem, object data)
    {
        var hash = JsonSignatureNormalizer.ComputeHashToByte(data);

        using StringReader stringReader = new(privateKeyPem);
        PemReader pemReader = new(stringReader);

        var privateKey = (AsymmetricKeyParameter)pemReader.ReadObject()
            ?? throw new ApplicationException("Invalid private key");

        // Set up the ECDSA signer with SHA-256
        ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, privateKey);

        // Feed the hash into the signer
        signer.BlockUpdate(hash, 0, hash.Length);

        // Generate the digital signature
        byte[] signature = signer.GenerateSignature();
        return Convert.ToBase64String(signature);
    }

    public bool VerifySignature(string publicKeyPem, object data, string signatureBase64)
    {
        var hash = JsonSignatureNormalizer.ComputeHashToByte(data);
        var signature = Convert.FromBase64String(signatureBase64);

        using StringReader stringReader = new(publicKeyPem);
        PemReader pemReader = new(stringReader);

        var publicKey = (AsymmetricKeyParameter)pemReader.ReadObject()
            ?? throw new ApplicationException("Invalid public key");

        ISigner verifier = SignerUtilities.GetSigner("SHA-256withECDSA");
        verifier.Init(false, publicKey);
        verifier.BlockUpdate(hash, 0, hash.Length);

        return verifier.VerifySignature(signature);
    }
}
