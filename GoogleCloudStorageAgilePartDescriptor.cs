using System;
using Ascentn.Workflow.Base;
using System.Collections;
using System.Collections.Generic;
using AgilePoint.Workflow.DataSources;
using Ascentn.AgilePart.Shared;
using System.Linq;
using static AgilePoint.AgilePart.GoogleCloudStorage.DesignTime.GoogleCloudStorageDTO;

namespace AgilePoint.AgilePart.GoogleCloudStorage
{

    public class GoogleCloudStorageAgilePartDescriptor : DataSourceDescriptorStorage
    {

        Hashtable m_AccessTokenDetails = null;

        #region Constructor

        public GoogleCloudStorageAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GoogleCloudStorageAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {
                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #endregion

        #region  Exposed Properties

        public string AppName
        {
            get
            {
                string appName = string.Empty;
                if (base[APPLICATION_NAME] != null)
                {
                    appName = base[APPLICATION_NAME].ToString();
                }
                return appName;
            }
            set
            {
                base[APPLICATION_NAME] = value;
            }
        }
        public string BucketName
        {
            get
            {
                string bucketname = string.Empty;
                if (base[nameof(BucketName)] != null)
                {
                    bucketname = base[nameof(BucketName)].ToString();
                }
                return bucketname;
            }
            set
            {
                base[nameof(BucketName)] = value;
            }
        }
        public string FolderName
        {
            get
            {
                string foldername = string.Empty;
                if (base[nameof(FolderName)] != null)
                {
                    foldername = base[nameof(FolderName)].ToString();
                }
                return foldername;
            }
            set
            {
                base[nameof(FolderName)] = value;
            }
        }
        public string FilePath
        {
            get
            {
                string m_FolderName = string.Empty;
                if (base[nameof(FilePath)] != null)
                {
                    m_FolderName = base[nameof(FilePath)].ToString();
                }
                return m_FolderName;
            }
            set
            {
                base[nameof(FilePath)] = value;
            }
        }
        public string FolderPath
        {
            get
            {
                string folderpath = string.Empty;
                if (base[nameof(FolderPath)] != null)
                {
                    folderpath = base[nameof(FolderPath)].ToString();
                }
                return folderpath;
            }
            set
            {
                base[nameof(FolderPath)] = value;
            }
        }
        public string ResponseTransformData
        {
            get
            {
                string text = string.Empty;
                if (base[nameof(ResponseTransformData)] != null)
                {
                    text = base[nameof(ResponseTransformData)].ToString();
                }
                return text;
            }
            set
            {
                base[nameof(ResponseTransformData)] = value;
            }
        }
        public string Email
        {
            get
            {
                string email = string.Empty;
                if (base[nameof(Email)] != null)
                {
                    email = base[nameof(Email)].ToString();
                }
                return email;
            }
            set
            {
                base[nameof(Email)] = value;
            }
        }
        public string Role
        {
            get
            {
                string role = string.Empty;
                if (base[nameof(Role)] != null)
                {
                    role = base[nameof(Role)].ToString();
                }
                return role;
            }
            set
            {
                base[nameof(Role)] = value;
            }
        }
        public string EmailCategory
        {
            get
            {
                string emailcategory = string.Empty;
                if (base[nameof(EmailCategory)] != null)
                {
                    emailcategory = base[nameof(EmailCategory)].ToString();
                }
                return emailcategory;
            }
            set
            {
                base[nameof(EmailCategory)] = value;
            }
        }
        #endregion

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
        }
        #endregion

        #region  DesignTime API
        private void InitializeAccessTokenSettings(CustomMethodParameter customParamobj)
        {
            Hashtable htParams = ShUtil.ToHashtable(customParamobj.Parameters.ToArray());
            if (htParams == null || htParams.Count == 0)
                throw new ArgumentNullException("Parameters");
            string appSettingKey = htParams["AppSettingKeyName"] + "";
            string appSettingName = htParams["AppSettingAppName"] + "";

            m_AccessTokenDetails = AgilePartHelper.GetAccessTokenConfiguration(customParamobj.WorkFlowAPI, customParamobj.AdminAPI, appSettingName, appSettingKey);
        }


        public List<NameValue> GetBucketList(CustomMethodParameter obj)
        {
            InitializeAccessTokenSettings(obj);
            GoogleCloudStorageOperation GCSOperation = new GoogleCloudStorageOperation(m_AccessTokenDetails);
            List<NameValue> bucketlist = GCSOperation.ListAllBuckets();
            return bucketlist;
        }
        public List<NameValue> GetSchemaMappingStructure(CustomMethodParameter obj)
        {
            InitializeAccessTokenSettings(obj);
            string operationName = obj.Parameters.Find(t => t.Name == "OperationName").Value.ToString();
            string processSchema = obj.Parameters.Find(t => t.Name == "ProcessSchema").Value.ToString();
            List<NameValue> schema = GetClassSchema(processSchema, operationName);
            return schema;
        }
        public List<NameValue> GetClassSchema(string processSchema, string operationName)
        {
            System.Type responseType = null;
            bool isResponse = true;
            switch (operationName)
            {
                case GoogleCloudStorageOperation.OperationGetBucket:
                    responseType = typeof(BucketDetails);
                    break;
                case GoogleCloudStorageOperation.OperationGetBucketACL:
                    responseType = typeof(List<BucketACL>);
                    break;
                case GoogleCloudStorageOperation.OperationGetObjectInformation:
                    responseType = typeof(FileInformation);
                    break;
                case GoogleCloudStorageOperation.OperationGetObjectACL:
                    responseType = typeof(List<FileACL>);
                    break;
                default:
                    throw new Exception("Invalid Operation Name");
            }
            SharedTransformUtility sharedTransformUtility = new SharedTransformUtility();
            List<NameValue> fileds = sharedTransformUtility.GenerateSchemaForClassProperties(processSchema, responseType, isResponse);
            return fileds;
        }

        #endregion
    }

    #region Create Bucket
    public class CreateBucketAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public CreateBucketAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public CreateBucketAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
        }
        #endregion
    }
    #endregion

    #region Create Folder
    public class CreateFolderAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public CreateFolderAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public CreateFolderAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region  Exposed Properties
        public string FolderID
        {
            get
            {
                string m_FolderName = string.Empty;
                if (base[nameof(FolderID)] != null)
                {
                    m_FolderName = base[nameof(FolderID)].ToString();
                }
                return m_FolderName;
            }
            set
            {
                base[nameof(FolderID)] = value;
            }
        }
        #endregion

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
            FolderName.ThrowIfNullOrEmpty(nameof(FolderName));
        }
        #endregion
    }
    #endregion

    #region Delete Bucekt
    public class DeleteBucketAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public DeleteBucketAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public DeleteBucketAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
        }
        #endregion
    }
    #endregion

    #region Delete Folder
    public class DeleteFolderAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public DeleteFolderAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public DeleteFolderAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion
    }
    #endregion

    #region Delete Object
    public class DeleteObjectAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public DeleteObjectAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public DeleteObjectAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion
    }
    #endregion

    #region Get Bucket
    public class GetBucketAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public GetBucketAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GetBucketAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
        }
        #endregion
    }
    #endregion

    #region Get Bucket ACL
    public class GetBucketACLAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public GetBucketACLAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GetBucketACLAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
        }
        #endregion
    }
    #endregion

    #region Get Object
    public class GetObjectAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public GetObjectAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GetObjectAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion

    }
    #endregion

    #region Get Object ACL
    public class GetObjectACLAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public GetObjectACLAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GetObjectACLAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion

    }
    #endregion

    #region Get Object Metadata

    public class GetObjectMetaDataAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public GetObjectMetaDataAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public GetObjectMetaDataAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region  Exposed Properties
        public string MetadataParameters
        {
            get
            {
                string metadataparameters = string.Empty;
                if (base[nameof(MetadataParameters)] != null)
                {
                    metadataparameters = base[nameof(MetadataParameters)].ToString();
                }
                return metadataparameters;
            }
            set
            {
                base[nameof(MetadataParameters)] = value;
            }
        }
        #endregion
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion

    }
    #endregion

    #region Set Bucket ACL
    public class SetBucketAclAgilePartDescriptor : CreateBucketAgilePartDescriptor
    {
        public SetBucketAclAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public SetBucketAclAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
            Email.ThrowIfNullOrEmpty(nameof(Email));
            EmailCategory.ThrowIfNullOrEmpty(nameof(EmailCategory));
            Role.ThrowIfNullOrEmpty(nameof(Role));
        }
        #endregion

    }
    #endregion

    #region Set Object ACL
    public class SetObjectACLAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public SetObjectACLAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public SetObjectACLAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
       

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
            Email.ThrowIfNullOrEmpty(nameof(Email));
            EmailCategory.ThrowIfNullOrEmpty(nameof(EmailCategory));
            Role.ThrowIfNullOrEmpty(nameof(Role));
        }
        #endregion
    }
    #endregion

    #region Set Object Metadata
    public class SetObjectMetaDataAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public SetObjectMetaDataAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public SetObjectMetaDataAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region  Exposed Properties
        public string MetadataParameters
        {
            get
            {
                string metadataparameters = string.Empty;
                if (base[nameof(MetadataParameters)] != null)
                {
                    metadataparameters = base[nameof(MetadataParameters)].ToString();
                }
                return metadataparameters;
            }
            set
            {
                base[nameof(MetadataParameters)] = value;
            }
        }
        #endregion

        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));
        }
        #endregion

    }
    #endregion

    #region Remove Object ACL

    public class RemoveObjectACLAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public RemoveObjectACLAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public RemoveObjectACLAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            EmailCategory.ThrowIfNullOrEmpty(nameof(EmailCategory));
            Email.ThrowIfNullOrEmpty(nameof(Email));
            FilePath.ThrowIfNullOrEmpty(nameof(FilePath));

        }
        #endregion

    }
    #endregion

    #region Remove Bucekt ACL
    public class RemoveBucketAclAgilePartDescriptor : GoogleCloudStorageAgilePartDescriptor
    {
        public RemoveBucketAclAgilePartDescriptor()
        {
            base.Synchronous = false;
        }
        public RemoveBucketAclAgilePartDescriptor(WFSocialOperation wFSocialOperation)
        {
            List<NameValue> parameters = wFSocialOperation.GetParametersAsNameValue();
            if (parameters != null)
            {

                Hashtable htParameter = ShUtil.ToHashtable(parameters.ToArray());
                foreach (DictionaryEntry item in htParameter)
                {
                    if (!string.IsNullOrEmpty(item.Key.ToString()) && item.Value != null)
                    {
                        base[item.Key.ToString()] = item.Value;
                    }
                }

            }
        }
        #region [ Validation methods ]
        public override void Validate()
        {
            base.Validate();
            EmailCategory.ThrowIfNullOrEmpty(nameof(EmailCategory));
            Email.ThrowIfNullOrEmpty(nameof(Email));
            BucketName.ThrowIfNullOrEmpty(nameof(BucketName));
        }
        #endregion

    }
    #endregion
}
