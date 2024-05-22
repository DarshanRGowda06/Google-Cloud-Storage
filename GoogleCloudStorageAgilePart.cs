using System;
using System.ComponentModel;
using Ascentn.Workflow.Base;
using System.Collections.Generic;
using Ascentn.AgilePart.Shared;

namespace AgilePoint.AgilePart.GoogleCloudStorage
{
    /// <summary>
    /// This class is AgilePart runtime class that will be invoked by AgilePoint Server.
    /// </summary>
    [AgilePart("GoogleCloudStorageAgilePart")]
    public class GoogleCloudStorageAgilePart : WFAgilePart
    {
        public List<NameValue> ParametersCollection = new List<NameValue>();
        [
        Description("Create Bucket"),
        AgilePartDescriptor("C43994EE-AEB4-4344-95B8-3D882197F33E", typeof(CreateBucketAgilePartDescriptor))
        ]
        public void CreateBucket(
                            WFProcessInstance pi,
                            WFAutomaticWorkItem w,
                            IWFAPI api,
                            NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationCreateBucket, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);

            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("CreateFolder"),AgilePartDescriptor("2C12AA50-C3B0-4882-86F9-CBEF7843DB21", typeof(CreateFolderAgilePartDescriptor))]
        public void CreateFolder(
                    WFProcessInstance pi,
                    WFAutomaticWorkItem w,
                    IWFAPI api,
                    NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationCreateFolder, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("Delete Bucket"), AgilePartDescriptor("FD05C141-2499-42F0-8AE9-D109BF322C3B", typeof(DeleteBucketAgilePartDescriptor))]
        public void DeleteBucket(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationDeleteBucket, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("Delete Folder"), AgilePartDescriptor("18C4424A-6B2C-456C-8C9E-B3CE8B14A0C2", typeof(DeleteFolderAgilePartDescriptor))]
        public void DeleteFolder(
             WFProcessInstance pi,
             WFAutomaticWorkItem w,
             IWFAPI api,
             NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationDeleteFolder, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("Delete Object"), AgilePartDescriptor("562D2897-5792-4D35-B380-20EC906FDF70", typeof(DeleteObjectAgilePartDescriptor))]
        public void DeleteObject(WFProcessInstance pi,WFAutomaticWorkItem w,IWFAPI api,NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationDeleteObject, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("Get Bucket"), AgilePartDescriptor("A6E7A9FF-EEED-49C1-91F9-7A936B7D323E", typeof(GetBucketAgilePartDescriptor))]
        public void GetBucket(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationGetBucket, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }
        [Description("GetBucketACL"), AgilePartDescriptor("0C747CB2-2640-47A8-B2C6-3A2B67EC0E26", typeof(GetBucketACLAgilePartDescriptor))]
        public void GetBucketACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationGetBucketACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("Get Object"), AgilePartDescriptor("5D5D189C-5D16-4AA3-A95B-AFE331234940", typeof(GetObjectAgilePartDescriptor))]
        public void GetObject(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationGetObjectInformation, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("GetObjectACL"), AgilePartDescriptor("C039CB8C-007A-4269-B73B-A0AF8ACC08BA", typeof(GetObjectACLAgilePartDescriptor))]
        public void GetObjectACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationGetObjectACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("GetObjectMetaData"), AgilePartDescriptor("2F32B927-AC7D-4B3C-B002-EC2FA4E8E9FC", typeof(GetObjectMetaDataAgilePartDescriptor))]
        public void GetObjectMetaData(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationGetObjectMetaData, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("RemoveBucketACL"), AgilePartDescriptor("E2F8083C-C1C9-474A-B60D-1968D0D0FA2E", typeof(RemoveBucketAclAgilePartDescriptor))]
        public void RemoveBucketACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationRemoveBucketACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }


        [Description("RemoveObjectACL"), AgilePartDescriptor("0DEFFBF9-6F49-4646-905D-1DD487344C68", typeof(RemoveObjectACLAgilePartDescriptor))]
        public void RemoveObjectACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationRemoveObjectACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("SetBucketACL"), AgilePartDescriptor("47B12824-0792-4A03-8CD3-F11BCF68BD1A", typeof(SetBucketAclAgilePartDescriptor))]
        public void SetBucketACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationSetBucketACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        [Description("SetObjectACL"), AgilePartDescriptor("72A69521-F358-44E8-883F-4BB703D04BC7", typeof(SetObjectACLAgilePartDescriptor))]
        public void SetObjectACL(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationSetObjectACL, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }
        [Description("SetObjectMetaData"), AgilePartDescriptor("96E9C6D4-4A78-49D7-858C-2403F4FA344A", typeof(SetObjectMetaDataAgilePartDescriptor))]
        public void SetObjectMetaData(WFProcessInstance pi, WFAutomaticWorkItem w, IWFAPI api, NameValue[] parameters)
        {
            try
            {
                string TenantID = string.Empty;
                if (pi.WFAdm != null && !String.IsNullOrEmpty(pi.WFAdm.TenantID))
                {
                    TenantID = pi.WFAdm.TenantID;
                }
                DropToQueue(pi, GoogleCloudStorageOperation.OperationSetObjectMetaData, w, parameters);
                if (w.Synchronous) MarkSuccess(api, pi, w, parameters);
            }
            catch (Exception ex)
            {
                HandleException(api, pi, w, parameters, ex);
            }
        }

        private void DropToQueue(WFProcessInstance pi, string operationName, WFAutomaticWorkItem autoWorkItem, NameValue[] parameters)
        {
            string queueName = "AP2GoogleCloudStorageQueue";
            List<NameValue> ParametersCollection = new List<NameValue>();
            string TenantID = string.Empty;
            if (pi.WFAdm != null && !string.IsNullOrEmpty(pi.WFAdm.TenantID))
            {
                TenantID = pi.WFAdm.TenantID;
            }

            WFSocialOperation operation = new WFSocialOperation(operationName, queueName, pi.ProcInstID, autoWorkItem.WorkItemID, pi.WorkObjectID, autoWorkItem.ActivityInstID, TenantID);

            foreach (var parameter in parameters)
            {
                ParametersCollection.Add(parameter);
            }

            operation.ParametersCollection = ParametersCollection;
            GoogleCloudStorageChannel operationChannel = new GoogleCloudStorageChannel(null,pi.WFApi, pi.WFAdm, pi.WorkObjectID, autoWorkItem, 5, 6, true);
            operationChannel.PerformOperation(operation);
        }

    }
}
