/*
* Slickflow 工作流引擎遵循LGPL协议，也可联系作者商业授权并获取技术支持；
* 除此之外的使用则视为不正当使用，请您务必避免由此带来的商业版权纠纷。
* 
The Slickflow project.
Copyright (C) 2014  .NET Workflow Engine Library

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Slickflow.Engine.Common;
using Slickflow.Engine.Utility;
using Slickflow.Data;
using Slickflow.Engine.Business.Entity;

namespace Slickflow.Engine.Business.Manager
{
    /// <summary>
    /// 流程定义管理类
    /// </summary>
    public class ProcessManager : ManagerBase
    {
        internal IXPDLReader XPDLReader
        {
            get;
            set;
        }

        #region 获取流程数据
        /// <summary>
        /// 根据流程GUID和版本标识获取流程
        /// </summary>
        /// <param name="processGUID"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public ProcessEntity GetByVersion(string processGUID, string version)
        {
            String sql = string.Empty;
            ProcessEntity entity = null;

            if (!string.IsNullOrEmpty(version))
            {
                sql = @"SELECT 
                            * 
                        FROM WfProcess 
                        WHERE ProcessGUID=@processGUID 
                            AND VERSION=@version";
            }
            else
            {
                sql = @"SELECT 
                            * 
                        FROM WfProcess 
                        WHERE ProcessGUID=@processGUID 
                            AND IsUsing=1";             //当前使用的版本
            }

            var list = Repository.Query<ProcessEntity>(sql, new { processGUID=processGUID, version=version})
                            .ToList<ProcessEntity>();

            if (list != null && list.Count() == 1)
            {
                entity = list[0];
            }
            else
            {
                throw new ApplicationException(string.Format(
                    "数据库没有对应的流程定义记录，ProcessGUID: {0}, Version: {1}", processGUID, version
                ));
            }
            return entity;
        }

        /// <summary>
        /// 获取所有流程记录
        /// </summary>
        /// <returns></returns>
        public List<ProcessEntity> GetAll()
        {
            var list = Repository.GetAll<ProcessEntity>().ToList<ProcessEntity>();
            return list;
        }
        #endregion

        #region 新增、更新和删除流程数据
        /// <summary>
        /// 新增流程记录
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(ProcessEntity entity)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                Repository.Insert<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.Commit();
            }
            catch (System.Exception)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 新增流程记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        public void Insert(IDbConnection conn, ProcessEntity entity, IDbTransaction trans)
        {
            Repository.Insert<ProcessEntity>(conn, entity, trans);
        }

        /// <summary>
        /// 更新流程记录
        /// </summary>
        /// <param name="entity"></param>
        public void Update(ProcessEntity entity)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                session.BeginTrans();
                Repository.Update<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.Commit();
            }
            catch (System.Exception)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 删除流程记录
        /// </summary>
        /// <param name="processGUID"></param>
        /// <param name="version"></param>
        public void Delete(string processGUID, string version)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                var entity = GetByVersion(processGUID, version);
                Repository.Delete<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.Commit();
            }
            catch (System.Exception)
            {
                session.Rollback();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 删除流程记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        public void Delete(IDbConnection conn, ProcessEntity entity, IDbTransaction trans)
        {
            Repository.Delete<ProcessEntity>(conn, entity, trans);
        }
        #endregion 

        #region 流程xml文件操作
        /// <summary>
        /// 保存XML文件
        /// </summary>
        /// <param name="entity"></param>
        internal void SaveProcessFile(ProcessFileEntity entity)
        {
            try
            {
                var processEntity = (new ProcessManager()).GetByVersion(entity.ProcessGUID, entity.Version);
                var filePath = processEntity.XmlFilePath;
                var serverPath = ConfigHelper.GetAppSettingString("WorkflowFileServer");
                var physicalFileName = serverPath + "\\" + filePath;
                var path = Path.GetDirectoryName(physicalFileName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(entity.XmlContent);
                xmlDoc.Save(physicalFileName);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(string.Format("保存流程定义XML文件失败，错误: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 读取流程XML文件内容
        /// </summary>
        /// <returns></returns>
        internal ProcessFileEntity GetProcessFile(string processGUID, string version)
        {
            var processEntity = GetByVersion(processGUID, version);
            XmlDocument xmlDoc = null;
            if (XPDLReader != null && XPDLReader.IsReadable() == true)
            {
                xmlDoc = XPDLReader.Read(processEntity);
            }
            else
            {
                xmlDoc = GetProcessXmlDocument(processEntity.XmlFilePath);
            }

            var processFileEntity = new ProcessFileEntity();

            processFileEntity.ProcessGUID = processEntity.ProcessGUID;
            processFileEntity.ProcessName = processEntity.ProcessName;
            processFileEntity.Version = processEntity.Version;
            processFileEntity.Description = processEntity.Description;
            processFileEntity.XmlContent = xmlDoc.OuterXml;
            return processFileEntity;
        }

        /// <summary>
        /// 读取流程的配置文件
        /// </summary>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        internal XmlDocument GetProcessXmlDocument(string filePath)
        {
            XmlDocument xmlDoc = null;

            //本地路径存储的文件
            string serverPath = ConfigHelper.GetAppSettingString("WorkflowFileServer");
            string physicalFileName = serverPath + "\\" + filePath;

            //检查文件是否存在
            if (!File.Exists(physicalFileName))
            {
                throw new ApplicationException(
                    string.Format("请配置流程XML文件，路径:{0}", physicalFileName)
                );
            }

            xmlDoc = new XmlDocument();
            xmlDoc.Load(physicalFileName);

            return xmlDoc;
        }
        #endregion
    }
}
