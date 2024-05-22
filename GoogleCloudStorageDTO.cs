using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgilePoint.AgilePart.GoogleCloudStorage.DesignTime
{
    public class GoogleCloudStorageDTO
    {
        #region GET BUCKET
      
        public class BucketPolicyOnly
        {
            public bool enabled { get; set; }
        }

        public class IamConfiguration
        {
            public BucketPolicyOnly bucketPolicyOnly { get; set; }
            public UniformBucketLevelAccess uniformBucketLevelAccess { get; set; }
            public string publicAccessPrevention { get; set; }
        }

        public class Label
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class BucketDetails
        {
            public string kind { get; set; }
            public string selfLink { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string projectNumber { get; set; }
            public string metageneration { get; set; }
            public string location { get; set; }
            public string storageClass { get; set; }
            public string etag { get; set; }
            public DateTime timeCreated { get; set; }
            public DateTime updated { get; set; }
            public List<Label> labels { get; set; }
            public SoftDeletePolicy softDeletePolicy { get; set; }
            public IamConfiguration iamConfiguration { get; set; }
            public string locationType { get; set; }
            public string rpo { get; set; }
        }

        public class SoftDeletePolicy
        {
            public string retentionDurationSeconds { get; set; }
            public DateTime effectiveTime { get; set; }
        }

        public class UniformBucketLevelAccess
        {
            public bool enabled { get; set; }
        }


        #endregion

        #region GET BUCKET ACL
        public class BucketACL
        {
            public string kind { get; set; }
            public string id { get; set; }
            public string selfLink { get; set; }
            public string bucket { get; set; }
            public string entity { get; set; }
            public string role { get; set; }
            public string etag { get; set; }
            public ProjectTeam projectTeam { get; set; }
            public string email { get; set; }
        }

        public class ProjectTeam
        {
            public string projectNumber { get; set; }
            public string team { get; set; }
        }


        #endregion

        #region GET FILE INFO
        public class FileInformation
        {
            public string kind { get; set; }
            public string id { get; set; }
            public string selfLink { get; set; }
            public string mediaLink { get; set; }
            public string name { get; set; }
            public string bucket { get; set; }
            public string generation { get; set; }
            public string metageneration { get; set; }
            public string contentType { get; set; }
            public string storageClass { get; set; }
            public string size { get; set; }
            public string md5Hash { get; set; }
            public string crc32c { get; set; }
            public string etag { get; set; }
            public DateTime timeCreated { get; set; }
            public DateTime updated { get; set; }
            public DateTime timeStorageClassUpdated { get; set; }
        }
        #endregion

        #region GET OBJECT ACL
        public class FileACL
        {
            public string kind { get; set; }
            public string @object { get; set; }
            public string generation { get; set; }
            public string id { get; set; }
            public string selfLink { get; set; }
            public string bucket { get; set; }
            public string entity { get; set; }
            public string role { get; set; }
            public string etag { get; set; }
            public ProjectTeam projectTeam { get; set; }
            public string email { get; set; }
        }
        #endregion

    }
}
