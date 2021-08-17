//using AkaConfig;
//using Amazon;
//using Amazon.S3;
//using Amazon.S3.Transfer;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//namespace BattleServer.BattleRecord
//{
//    public class AWSWriter : IWriter
//    {
//        public readonly string bucket;

//        public AWSWriter()
//        {
//            AWSConfigs.AWSProfilesLocation = Config.ConfigRootPath + "Cloud/" + Config.BattleServerConfig.RecordSetting.AWSCredential;
//            AWSConfigs.AWSRegion = Config.BattleServerConfig.RecordSetting.AWSRegion;
//            AWSConfigs.AWSProfileName = Config.BattleServerConfig.RecordSetting.AWSProfile;
//            bucket = Config.BattleServerConfig.RecordSetting.AWSBucket;
//        }

//        public async Task<bool> WriteAsync(MemoryStream stream, string recordKey)
//        {
//            bool result = true;
//            recordKey = $"records/{DateTime.Now.ToString("yyyyMMdd")}/{recordKey}";

//            stream.Seek(0, SeekOrigin.Begin);
//            try
//            {
//                using (var s3Client = new AmazonS3Client() )
//                {
//                    using (var transferUtility = new TransferUtility(s3Client))
//                    {
//                        var transferUtilityReq = new TransferUtilityUploadRequest
//                        {
//                            BucketName = bucket,
//                            InputStream = stream,
//                            StorageClass = S3StorageClass.Standard,
//                            PartSize = stream.Length,
//                            Key = recordKey,
//                            CannedACL = S3CannedACL.PublicRead
//                        };

//                        await transferUtility.UploadAsync(transferUtilityReq);
//                    }
//                }
//            }
//            catch (AmazonS3Exception s3e)
//            {
//                AkaLogger.Log.Debug.Exception("Record.AWS.S3", s3e);
//                AkaLogger.Logger.Instance().Error("[Record.AWS.S3] "+ s3e.ToString());
//                result = false;
//            }
//            catch (Exception e)
//            {
//                AkaLogger.Log.Debug.Exception("Record.AWS.Unknown", e);
//                AkaLogger.Logger.Instance().Error("[Record.AWS.Unknown] " + e.ToString());
//                result = false;
//            }
//            return result;
//        }
//    }

//}