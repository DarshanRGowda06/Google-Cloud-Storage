using Ascentn.AgilePart.Shared;
using Ascentn.Workflow.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static AgilePoint.AgilePart.GoogleCloudStorage.DesignTime.GoogleCloudStorageDTO;

namespace AgilePoint.AgilePart.GoogleCloudStorage
{
    public class GoogleCloudStorageChannel : IWFAsyncAgilePart
    {
        readonly IWFAPI m_api = null;
        readonly IWFAdmin m_admin = null;
        readonly int m_NumOfRetries;
        readonly int m_DelayInRetries;
        readonly TraceLogger m_TraceLoggerConnector = null;
        readonly AutoResetEvent m_Signal = new AutoResetEvent(false);
        GoogleCloudStorageOperation GCLOperation = null;
        Hashtable m_AccessTokenDetails = null;
        AgilePartDebugLog agilePartDebugLog = null;
        readonly string m_workObjID;
        readonly WFAutomaticWorkItem m_autoWorkItem = null;
        public GoogleCloudStorageChannel(string accessToken, IWFAPI api, IWFAdmin admin, string workObjectId, WFAutomaticWorkItem autoWorkItem, int NumOfRetries, int DelayInRetries, bool TraceMode)
        {
            m_api = api;
            m_admin = admin;
            m_NumOfRetries = NumOfRetries;
            m_DelayInRetries = DelayInRetries;
            m_TraceLoggerConnector = new TraceLogger(TraceMode);
            m_workObjID = workObjectId;
            m_autoWorkItem = autoWorkItem;
        }
        public void PerformOperation(IWFAgilePartOperation operation)
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            WFSocialOperation currentOperation = (WFSocialOperation)operation;
            agilePartDebugLog = new AgilePartDebugLog(currentOperation);
            InitializeAccessTokenSettings(currentOperation);
            GCLOperation = new GoogleCloudStorageOperation(m_AccessTokenDetails, currentOperation);
            try
            {

                // Define a dictionary to map operation names to methods
                Dictionary<string, Action<WFSocialOperation>> operationMap = new Dictionary<string, Action<WFSocialOperation>>
                {
                            {GoogleCloudStorageOperation.OperationCreateBucket, op => CreateBucket(op)},
                            {GoogleCloudStorageOperation.OperationCreateFolder, op => CreateFolder(op)},
                            {GoogleCloudStorageOperation.OperationDeleteBucket, op => DeleteBucket(op)},
                            {GoogleCloudStorageOperation.OperationDeleteObject, op => DeleteObject(op)},
                            {GoogleCloudStorageOperation.OperationDeleteFolder, op => DeleteFolder(op)},
                            {GoogleCloudStorageOperation.OperationGetBucket, op => GetBucket(op)},
                            {GoogleCloudStorageOperation.OperationGetBucketACL, op => GetBucketACL(op)},
                            {GoogleCloudStorageOperation.OperationGetObjectInformation, op => GetObjectInformation(op)},
                            {GoogleCloudStorageOperation.OperationGetObjectACL, op => GetObjectACL(op)},
                            {GoogleCloudStorageOperation.OperationGetObjectMetaData, op => GetObjectMetadata(op)},
                            {GoogleCloudStorageOperation.OperationSetBucketACL, op => SetBucketACL(op)},
                            {GoogleCloudStorageOperation.OperationSetObjectACL, op => SetObjectACL(op)},
                            {GoogleCloudStorageOperation.OperationSetObjectMetaData, op => SetObjectMetadata(op)},
                            {GoogleCloudStorageOperation.OperationRemoveBucketACL, op => RemoveBucketACL(op)},
                            {GoogleCloudStorageOperation.OperationRemoveObjectACL, op => RemoveObjectACL(op)},
                         
                };

                // Execute the operation based on the current operation name
                if (operationMap.TryGetValue(currentOperation.OperationName, out Action<WFSocialOperation> socialoperation))
                {
                    socialoperation(currentOperation);
                }
                else
                {
                    throw new InvalidOperationException(currentOperation.OperationName);
                }

                SharedDescriptor.MarkAgilePartWorkItemAsSuccess(currentOperation, m_api);
                m_TraceLoggerConnector.TraceWriteLine(currentOperation.TenantID, MessageType.Information, $"ThreadID({threadID}): Operation {currentOperation.OperationName} completed successfully");
            }
            catch (Exception ex)
            {
                m_TraceLoggerConnector.TraceWriteLine(operation.TenantID, MessageType.Information, $"Auto WorkItem ID: {currentOperation.WorkItemId} Activity Instance ID: {currentOperation.ActivityInstID} Process Instance ID:{currentOperation.ProcessInstanceId} ThreadID({threadID}): Failed to perform operation: {currentOperation.OperationName}.");
                m_TraceLoggerConnector.TraceWriteLine(operation.TenantID, MessageType.Error, ex, $"ThreadID({threadID}): Failed to perform operation:{currentOperation.OperationName}", threadID, currentOperation.OperationName);
                SharedDescriptor.MarkAgilePartWorkItemAsFailure(currentOperation, ex, string.Empty, m_api);
                throw;
            }
            finally
            {
                m_Signal.Set();
            }
        }

        #region Constructor

        private void InitializeAccessTokenSettings(WFSocialOperation currentOperation)
        {
            NameValue[] parametersList = currentOperation.GetParametersAsNameValue().ToArray();
            Hashtable htParams = ShUtil.ToHashtable(parametersList);
            if (htParams == null || htParams.Count == 0)
                throw new ArgumentNullException("Parameters");
            string appSettingKey = htParams["AppSettingKey"] + "";
            string appSettingName = htParams["AppName"] + "";
            m_AccessTokenDetails = AgilePartHelper.GetAccessTokenConfiguration(m_api, m_admin, appSettingName, appSettingKey);
        }
        #endregion

        #region Create bucket
        private void CreateBucket(WFSocialOperation operation)
        {
            CreateBucketAgilePartDescriptor agilePartDescriptor = new CreateBucketAgilePartDescriptor(operation);
            DebugLog($"CreateBucket Started:{agilePartDescriptor.BucketName}");
            agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
            GCLOperation.CreateBucket(agilePartDescriptor.BucketName);
            DebugLog($"CreateBucket Ended:{agilePartDescriptor.BucketName}");
        }
        #endregion

        #region Create Folder
        private void CreateFolder(WFSocialOperation operation)
        {
            CreateFolderAgilePartDescriptor agilePartDescriptor = new CreateFolderAgilePartDescriptor(operation);
            DebugLog($"CreateFolder Started:{agilePartDescriptor.FolderName}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            agilePartDescriptor.FolderName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FolderName));
            GCLOperation.CreateFolder(agilePartDescriptor.FolderName, agilePartDescriptor.FilePath);
            m_api.SetCustomAttr(operation.WorkObjectId, agilePartDescriptor.FolderID, agilePartDescriptor.FilePath + "/" + agilePartDescriptor.FolderName);
            DebugLog($"CreateFolder Ended:{agilePartDescriptor.FolderName}");
        }
        #endregion

        #region Delete Bucket
        private void DeleteBucket(WFSocialOperation operation)
        {
            DeleteBucketAgilePartDescriptor agilePartDescriptor = new DeleteBucketAgilePartDescriptor(operation);
            DebugLog($"DeleteBucket Started:{agilePartDescriptor.BucketName}");
            agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
            GCLOperation.DeleteBucket(agilePartDescriptor.BucketName);
            DebugLog($"DeleteBucket Ended:{agilePartDescriptor.BucketName}");
            
        }
        #endregion

        #region Delete Folder
        private void DeleteFolder(WFSocialOperation operation)
        {
            DeleteFolderAgilePartDescriptor agilePartDescriptor = new DeleteFolderAgilePartDescriptor(operation);
            DebugLog($"DeleteFolder Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            GCLOperation.DeleteFolder(agilePartDescriptor.FilePath);
            DebugLog($"DeleteFolder Ended:{agilePartDescriptor.FilePath}");
        }

        #endregion

        #region Delete Object
        private void DeleteObject(WFSocialOperation operation)
        {
            DeleteObjectAgilePartDescriptor agilePartDescriptor = new DeleteObjectAgilePartDescriptor(operation);
            DebugLog($"DeleteObject Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            GCLOperation.DeleteObject(agilePartDescriptor.FilePath);
            DebugLog($"DeleteObject Ended:{agilePartDescriptor.FilePath}");

        }
        #endregion

        #region Get Bucket
        private void GetBucket(WFSocialOperation operation)
        {
            try
            {
                GetBucketAgilePartDescriptor agilePartDescriptor = new GetBucketAgilePartDescriptor(operation);
                DebugLog($"GetBucket Started:{agilePartDescriptor.BucketName}");
                agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
                BucketDetails response = GCLOperation.GetBucket(agilePartDescriptor.BucketName);
                DebugLog($"SetResponseToSchema Started");
                SharedTransformUtility.SetResponseToSchema(operation, response, m_api, string.Empty);
                DebugLog($"SetResponseToSchema Ended");
                DebugLog($"GetBucket Ended:{agilePartDescriptor.BucketName}");

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Bucket ACL
        private void GetBucketACL(WFSocialOperation operation)
        {
            try
            {
                GetBucketACLAgilePartDescriptor agilePartDescriptor = new GetBucketACLAgilePartDescriptor(operation);
                DebugLog($"GetBucketACL Started:{agilePartDescriptor.BucketName}");
                agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
                List<NameValue> response = GCLOperation.GetBucketACL(agilePartDescriptor.BucketName);
                var ResponseData = response.Select(item => new { item.Name, item.Value }).FirstOrDefault();
                DebugLog($"SetResponseToSchema Started");
                SharedTransformUtility.SetResponseToSchema(operation, ResponseData.Value.ToString(), ResponseData.Name.ToString(), m_api);
                DebugLog($"SetResponseToSchema Ended");
                DebugLog($"GetBucketACL Ended:{agilePartDescriptor.BucketName}");
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Object Info
        private void GetObjectInformation(WFSocialOperation operation)
        {
            GetObjectAgilePartDescriptor agilePartDescriptor = new GetObjectAgilePartDescriptor(operation);
            DebugLog($"GetFileInformation Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            FileInformation response = GCLOperation.GetObjectInformation(agilePartDescriptor.FilePath);
            DebugLog($"SetResponseToSchema Started");
            SharedTransformUtility.SetResponseToSchema(operation, response, m_api, string.Empty);
            DebugLog($"SetResponseToSchema Ended");
            DebugLog($"GetFileInformation Ended:{agilePartDescriptor.FilePath}");
        }
        #endregion

        #region Get Object ACL
        private void GetObjectACL(WFSocialOperation operation)
        {
            GetObjectACLAgilePartDescriptor agilePartDescriptor = new GetObjectACLAgilePartDescriptor(operation);
            DebugLog($"GetFileACL Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            List<NameValue> response = GCLOperation.GetObjectACL(agilePartDescriptor.FilePath);
            var ResponseData = response.Select(item => new { item.Name, item.Value }).FirstOrDefault();
            DebugLog($"SetResponseToSchema Started");
            SharedTransformUtility.SetResponseToSchema(operation, ResponseData.Value.ToString(), ResponseData.Name.ToString(), m_api);
            DebugLog($"SetResponseToSchema Ended");
            DebugLog($"GetFileACL Ended:{agilePartDescriptor.FilePath}");
        }
        #endregion

        #region Get Object Metadata
        private void GetObjectMetadata(WFSocialOperation operation)
        {
            GetObjectMetaDataAgilePartDescriptor agilePartDescriptor = new GetObjectMetaDataAgilePartDescriptor(operation);
            DebugLog($"GetObjectMetadata Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            List<NameValue> metaDataParamters = ShUtil.JsonDeserialize<List<NameValue>>(agilePartDescriptor.MetadataParameters);
                if (metaDataParamters != null && metaDataParamters.Count > 0)
                {
                    List<NameValue> resultParamters = GCLOperation.GetObjectMetaData(agilePartDescriptor.FilePath);
                    List<NameValue> finalResult = (from NameValue nameValue in resultParamters
                                                   from NameValue input in metaDataParamters
                                                   where nameValue.Name.ToLower() == input.Name.ToLower()
                                                   select new NameValue(input.Value.ToString(), nameValue.Value.ToString())).ToList();

                    m_api.SetCustomAttrs(operation.WorkObjectId, finalResult.ToArray());
                    DebugLog($"GetObjectMetadata Ended:{agilePartDescriptor.FilePath}");
                }
                else
                {
                    throw new InvalidDataException("Provided Key Values are empty");
                }

           
        }
        #endregion

        #region Set Buket ACL
        private void SetBucketACL(WFSocialOperation operation)
        {
            SetBucketAclAgilePartDescriptor agilePartDescriptor = new SetBucketAclAgilePartDescriptor(operation);
            DebugLog($"SetBucketACL Started:{agilePartDescriptor.BucketName}");
            agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
            agilePartDescriptor.EmailCategory.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.EmailCategory));
            agilePartDescriptor.Email.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Email));
            agilePartDescriptor.Role.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Role));
            GCLOperation.SetBucketACL(agilePartDescriptor.BucketName, agilePartDescriptor.EmailCategory, agilePartDescriptor.Email, agilePartDescriptor.Role);
            DebugLog($"SetBucketACL Ended:{agilePartDescriptor.BucketName}");
        }
        #endregion

        #region Set Object ACL
        private void SetObjectACL(WFSocialOperation operation)
        {
            SetObjectACLAgilePartDescriptor agilePartDescriptor = new SetObjectACLAgilePartDescriptor(operation);
            DebugLog($"SetObjectACL Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            agilePartDescriptor.EmailCategory.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.EmailCategory));
            agilePartDescriptor.Email.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Email));
            agilePartDescriptor.Role.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Role));
            GCLOperation.SetObjectACL(agilePartDescriptor.FilePath, agilePartDescriptor.EmailCategory, agilePartDescriptor.Email, agilePartDescriptor.Role);
            DebugLog($"SetObjectACL Ended:{agilePartDescriptor.FilePath}");

        }

        #endregion

        #region Set Object Metadata

        private void SetObjectMetadata(WFSocialOperation operation)
        {
            SetObjectMetaDataAgilePartDescriptor agilePartDescriptor = new SetObjectMetaDataAgilePartDescriptor(operation);
            DebugLog($"SetObjectMetadata Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));

                List<NameValue> metaDataParamters = ShUtil.JsonDeserialize<List<NameValue>>(agilePartDescriptor.MetadataParameters);
                if (metaDataParamters != null && metaDataParamters.Count > 0)
                {
                    int index = 0;
                    List<NameValue> resultParamters = GCLOperation.GetObjectMetaData(agilePartDescriptor.FilePath);
                    foreach (var metaData in metaDataParamters)
                    {
                        if (!string.IsNullOrEmpty(metaData.Name) && !string.IsNullOrEmpty(metaData.Value + ""))
                        {
                            if (resultParamters.Any(x => x.Name.ToLower().Equals(metaData.Name.ToLower())))
                            {
                                index = resultParamters.FindIndex(x => x.Name.ToLower().Equals(metaData.Name.ToLower()));
                                resultParamters[index].Value = metaData.Value;
                            }
                            else
                            {
                                resultParamters.Add(metaData);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Provided Key Values are empty");
                        }
                    }
                    GCLOperation.SetObjectMetaData(agilePartDescriptor.FilePath, resultParamters);
                    DebugLog($"SetObjectMetadata Ended:{agilePartDescriptor.FilePath}");
                }
                else
                {
                    throw new InvalidDataException("Provided Key Values are empty");
                }
           
        }
        #endregion

        #region Remove Bucket ACL

        private void RemoveBucketACL(WFSocialOperation operation)
        {
            RemoveBucketAclAgilePartDescriptor agilePartDescriptor = new RemoveBucketAclAgilePartDescriptor(operation);
            DebugLog($"RemoveBucketACL Started:{agilePartDescriptor.BucketName}");
            agilePartDescriptor.BucketName.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.BucketName));
            agilePartDescriptor.EmailCategory.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.EmailCategory));
            agilePartDescriptor.Email.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Email));
            GCLOperation.RemoveBucketACL(agilePartDescriptor.BucketName, agilePartDescriptor.EmailCategory, agilePartDescriptor.Email);
            DebugLog($"RemoveBucketACL Ended:{agilePartDescriptor.FilePath}");

        }
        #endregion

        #region Remove Object ACL
        private void RemoveObjectACL(WFSocialOperation operation)
        {
            RemoveObjectACLAgilePartDescriptor agilePartDescriptor = new RemoveObjectACLAgilePartDescriptor(operation);
            DebugLog($"RemoveObjectACL Started:{agilePartDescriptor.FilePath}");
            agilePartDescriptor.FilePath.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.FilePath));
            agilePartDescriptor.EmailCategory.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.EmailCategory));
            agilePartDescriptor.Email.ThrowIfNullOrEmpty(nameof(agilePartDescriptor.Email));
            GCLOperation.RemoveObjectACL(agilePartDescriptor.FilePath, agilePartDescriptor.EmailCategory, agilePartDescriptor.Email);
            DebugLog($"RemoveObjectACL Ended:{agilePartDescriptor.FilePath}");
        }
        #endregion

        #region Logger
        private void DebugLog(string LogMessage, [CallerMemberName] string MemberName = "")
        {
            agilePartDebugLog.WriteDebugLog(LogMessage, MemberName);
        }
        #endregion
    }
}