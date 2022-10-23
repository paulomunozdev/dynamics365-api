using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using System.ComponentModel;

namespace DAL
{
    public abstract class CRM
    {
        public class Status
        {
            public int StateCode { get; set; }
            public int StatusCode { get; set; }
            public string Name { get; set; }
        }

        public enum EstadosOportunidad
        {
            [Description("Win")]
            Win = 3,
            [Description("Lose")]
            Lose = 4,

        }
        public string EntityName { get; set; }

        protected IOrganizationService service { get; set; }
        protected ITracingService traceService { get; set; }


        //public CRM(string EntityName, string xrmConnectionStringNamePrefix)
        //{
        //    this.EntityName = EntityName;
        //    this.service = JumpStart.CrmConnectionFactory.GetOrganizationService(xrmConnectionStringNamePrefix);
        //}
        public CRM(string EntityName)
        {
            this.EntityName = EntityName;
            this.service = JumpStartDATA.CrmConnectionFactory.GetNewOrganizationService();
        }
        public CRM(string EntityName, IOrganizationService svc)
        {
            this.EntityName = EntityName;
            this.service = svc;
        }
        public CRM(string EntityName, IOrganizationService svc, ITracingService traceService)
        {
            this.EntityName = EntityName;


            if (svc == null)
            {

                this.service = JumpStartDATA.CrmConnectionFactory.GetNewOrganizationService();
            }
            else
            {
                this.service = svc;
            }


            this.traceService = traceService;

        }
        public CRM(IOrganizationService svc)
        {
            this.service = svc;
        }
        public CRM(IOrganizationService svc, ITracingService traceService)
        {
            if (svc == null)
            {

                this.service = JumpStartDATA.CrmConnectionFactory.GetNewOrganizationService();
            }
            else
            {
                this.service = svc;
            }
            this.traceService = traceService;
        }


        public CRM()
        {
            this.EntityName = String.Empty;
            this.service = JumpStartDATA.CrmConnectionFactory.GetNewOrganizationService();
        }



        public void ChangeStatus(Guid guid, int stateCode, int statusCode)
        {
            if (statusCode != 3)
            {
                var entidad = new Entity(EntityName);
                entidad.Id = guid;
                entidad.Attributes["statecode"] = new OptionSetValue(stateCode);
                entidad.Attributes["statuscode"] = new OptionSetValue(statusCode);

                UpdateRequest setStateRequest = new UpdateRequest();
                setStateRequest.Target = entidad;

                service.Execute(setStateRequest);
            }
            else
            {
                OpportunityWin(guid);
            }


            //****Metodo antes Actualización IU*****
            //SetStateRequest setStateRequest = new SetStateRequest()
            //{
            //    EntityMoniker = new EntityReference
            //    {
            //        Id = guid,
            //        LogicalName = EntityName,
            //    },
            //    State = new OptionSetValue(stateCode),
            //    Status = new OptionSetValue(statusCode)
            //};

            //service.Execute(setStateRequest);
        }

        public void OpportunityWin(Guid oportunidad)
        {

            var winOppRequest = new WinOpportunityRequest();
            Entity opportunityClose = new Entity("opportunityclose");
            opportunityClose["opportunityid"] = new EntityReference("opportunity", oportunidad);
            winOppRequest.OpportunityClose = opportunityClose;
            winOppRequest.RequestName = "WinOpportunity";
            OptionSetValue oStatus = new OptionSetValue();
            oStatus.Value = (int)EstadosOportunidad.Win;
            winOppRequest.Status = oStatus;
            service.Execute(winOppRequest);

        }

        public void OpportunityLose(Guid oportunidad)
        {

            var loseOppRequest = new LoseOpportunityRequest();
            Entity opportunityClose = new Entity("opportunityclose");
            opportunityClose["opportunityid"] = new EntityReference("opportunity", oportunidad);
            loseOppRequest.OpportunityClose = opportunityClose;
            loseOppRequest.RequestName = "LoseOpportunity";
            OptionSetValue oStatus = new OptionSetValue();
            oStatus.Value = (int)EstadosOportunidad.Lose;
            loseOppRequest.Status = oStatus;
            service.Execute(loseOppRequest);

        }

        public Entity getById(Guid id)
        {
            return service.Retrieve(EntityName, id, new ColumnSet(true));
        }

        public Entity getById(Guid id, params string[] str)
        {
            return service.Retrieve(EntityName, id, new ColumnSet(str));
        }

        public Guid Create(Entity entity)
        {
            return service.Create(entity);
        }

        public Entity GetByCustomId(string customIdName, object customIdValue, params string[] columns)
        {
            var query = GetQuery();

            query.Criteria.AddCondition(customIdName, ConditionOperator.Equal, customIdValue);
            query.TopCount = 1;
            query.AddOrder("modifiedon", OrderType.Descending);

            if (columns.Count() > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            var ec = service.RetrieveMultiple(query);

            if (ec.Entities.Count > 0)
            {
                return ec.Entities.First();
            }
            else
            {
                return null;
            }
        }

        public void Update(Entity entity)
        {
            service.Update(entity);
        }

        public QueryExpression GetQuery()
        {
            var query = new QueryExpression(EntityName);

            query.NoLock = true;

            return query;
        }

        public void Assign(EntityReference target, EntityReference assignee)
        {
            var entidad = new Entity(target.LogicalName);
            entidad.Id = target.Id;
            entidad.Attributes["ownerid"] = assignee;
            UpdateRequest req = new UpdateRequest();
            req.Target = entidad;

            //****Metodo antes Actualización IU****
            //var req = new AssignRequest();

            //req.Target = target;

            //req.Assignee = assignee;

            //service.Execute(req);
        }

        public EntityCollection GetQuery(QueryExpression query)
        {
            return service.RetrieveMultiple(query);
        }

        public void PagingHelper(QueryExpression query, Action<EntityCollection, object[]> action, params object[] parms)
        {
            query.PageInfo.PageNumber = 1;
            query.PageInfo.PagingCookie = null;

            var moreRecords = true;

            while (moreRecords)
            {
                var ec = service.RetrieveMultiple(query);

                action(ec, parms);

                moreRecords = ec.MoreRecords;

                if (moreRecords)
                {
                    ++query.PageInfo.PageNumber;

                }
            }
        }

        public EntityCollection RetrieveMultipleAllPages(QueryExpression query)
        {
            query.PageInfo.PageNumber = 1;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.ReturnTotalRecordCount = true;
            EntityCollection ecReturn = new EntityCollection();
            var ec = service.RetrieveMultiple(query);
            var moreRecords = ec.MoreRecords;
            ecReturn.Entities.AddRange(ec.Entities);
            //var moreRecords = true;

            while (moreRecords)
            {
                ec = service.RetrieveMultiple(query);
                ecReturn.Entities.AddRange(ec.Entities);
                moreRecords = ec.MoreRecords;

                if (moreRecords)
                {
                    ++query.PageInfo.PageNumber;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }



            }

            return ecReturn;
        }

        public void PagingHelper(QueryExpression query, Action<EntityCollection> action)
        {
            query.PageInfo.PageNumber = 1;
            query.PageInfo.PagingCookie = null;

            var moreRecords = true;

            while (moreRecords)
            {
                var ec = service.RetrieveMultiple(query);

                action(ec);

                moreRecords = ec.MoreRecords;

                if (moreRecords)
                {
                    ++query.PageInfo.PageNumber;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }
            }
        }


        public void PagingHelperThreaded(QueryExpression query, Action<EntityCollection> action)
        {
            var tasks = new List<System.Threading.Tasks.Task>();

            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = 500;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.ReturnTotalRecordCount = true;

            var moreRecords = true;

            while (moreRecords)
            {
                var ec = service.RetrieveMultiple(query);

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() => { action(ec); }));

                moreRecords = ec.MoreRecords;

                if (moreRecords)
                {
                    ++query.PageInfo.PageNumber;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        }

        public void PagingHelperThreaded(QueryExpression query, Action<EntityCollection, object[]> action, params object[] parms)
        {
            var tasks = new List<System.Threading.Tasks.Task>();

            query.PageInfo.Count = 500;
            query.PageInfo.PageNumber = 1;
            query.PageInfo.PagingCookie = null;

            var moreRecords = true;

            while (moreRecords)
            {
                var ec = service.RetrieveMultiple(query);

                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() => { action(ec, parms); }));

                moreRecords = ec.MoreRecords;

                if (moreRecords)
                {
                    ++query.PageInfo.PageNumber;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        }

        protected void RefreshService()
        {
            this.service = JumpStartDATA.CrmConnectionFactory.GetNewOrganizationService();
        }


        public bool AssigneeTo(Guid baseId, string entityNameAssignee, Guid assigneeId)
        {
            bool ejecucionValida = false;
            try
            {
                AssignRequest request = new AssignRequest();
                request.Assignee = new EntityReference(entityNameAssignee, assigneeId);
                request.Target = new EntityReference(this.EntityName, baseId);
                var result = this.service.Execute(request);

                ejecucionValida = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return ejecucionValida;
        }

        #region UTIL
        public String EntityAttibuteString(Entity entity, string attibute)
        {
            return entity.Contains(attibute) ? entity.GetAttributeValue<string>(attibute) : "";
        }
        public bool EntityAttibuteBool(Entity entity, string attibute)
        {
            return entity.Contains(attibute) ? entity.GetAttributeValue<bool>(attibute) : false;
        }
        public int? EntityAttibuteInt(Entity entity, string attibute)
        {
            int? _return = null;
            if (entity.Contains(attibute))
                _return = entity.GetAttributeValue<int>(attibute);
            return _return;
        }
        public DateTime? EntityAttibuteDate(Entity entity, string attibute)
        {
            DateTime? _return = null;
            if (entity.Contains(attibute))
                _return = entity.GetAttributeValue<DateTime>(attibute);
            return _return;
        }
        public int? EntityAttibutePicklist(Entity entity, string attibute)
        {
            int? _return = null;
            if (entity.Contains(attibute))
                return (entity.GetAttributeValue<OptionSetValue>(attibute).Value);
            return _return;
        }
        public EntityReference EntityAttibuteEntityReference(Entity entity, string attibute)
        {
            EntityReference _return = null;
            if (entity.Contains(attibute))
                _return = entity.GetAttributeValue<EntityReference>(attibute);
            return _return;
        }
        public Entity Copy(Entity record, params string[] columns)
        {
            var result = new Entity(record.LogicalName, record.Id);

            var columnsToCopy = columns;

            if (columnsToCopy.Length == 0)
            {
                columnsToCopy = record.Attributes.Keys.ToArray();
            }

            foreach (var column in columnsToCopy)
            {
                if (record.Contains(column))
                    result[column] = record[column];
            }

            return result;

        }

        public object[] ListGuiToObject(IList<Guid> listGuid)
        {
            var _returnValue = new object[listGuid.Count];
            var i = 0;
            foreach (var item in listGuid)
            {
                _returnValue[i] = item;
                i++;
            }
            return _returnValue;
        }

        #endregion UTIL
        public bool CompareEntity(Entity entityOrg, Entity entityNew, List<string> fields, bool fieldsNotToCompare = false)
        {
            bool sonIguales = true;

            if (entityOrg != null)
            {
                Entity entityOrgComp = new Entity(this.EntityName);
                Entity entityNewComp = new Entity(this.EntityName);

                //entity original - tomamos los atributos de la nueva que es la a comparar ya que pueden ser menos campos debido a que se actualiza.
                foreach (var attribute in entityOrg.Attributes.OrderBy(x => x.Key))
                {
                    bool campoValido = false;

                    //si es la lista de campos a no comprar
                    if (fieldsNotToCompare)
                    {
                        campoValido = !fields.Contains(attribute.Key) && entityNew.Attributes.Contains(attribute.Key);
                    }
                    else
                    {
                        campoValido = fields.Contains(attribute.Key) && entityNew.Attributes.Contains(attribute.Key);
                    }

                    if ((attribute.Key != null | attribute.Value != null)
                            && attribute.Key != "id" && campoValido)
                    {
                        entityOrgComp.Attributes.Add(attribute.Key, attribute.Value);
                    }
                }

                List<string> noEnOriginal = new List<string>();

                if (fieldsNotToCompare)
                {
                    var _noEnOriginalTemp = entityNew.Attributes.Where(x => !entityOrgComp.Attributes.Select(a => a.Key).Contains(x.Key)).ToList();
                    _noEnOriginalTemp = _noEnOriginalTemp.Where(x => x.Value != null).ToList();
                    noEnOriginal = _noEnOriginalTemp.Where(x => x.Value.ToString() != string.Empty).Select(a => a.Key).ToList();

                    noEnOriginal = noEnOriginal.Where(x => !fields.Contains(x)).ToList();
                }
                else
                {
                    noEnOriginal = entityNew.Attributes.Where(x => !entityOrgComp.Attributes.Select(a => a.Key).Contains(x.Key)
                                                                  && x.Value != string.Empty).Select(x => x.Key).ToList();
                }

                if (noEnOriginal.Count == 0)
                {
                    //entity nueva
                    foreach (var attribute in entityNew.Attributes.OrderBy(x => x.Key))
                    {
                        bool campoValido = false;

                        //si es la lista de campos a no comprar
                        if (fieldsNotToCompare)
                        {
                            campoValido = !fields.Contains(attribute.Key) && !noEnOriginal.Contains(attribute.Key);
                        }
                        else
                        {
                            campoValido = fields.Contains(attribute.Key) && !noEnOriginal.Contains(attribute.Key);
                        }

                        if ((attribute.Key != null | attribute.Value != null)
                                && attribute.Key != "id" && campoValido)
                        {
                            entityNewComp.Attributes.Add(attribute.Key, attribute.Value);
                        }
                    }

                    //var valor1 = JsonConvert.SerializeObject(entityOrgComp);
                    //var valor2 = JsonConvert.SerializeObject(entityNewComp);

                    foreach (var key in entityNewComp.Attributes.Keys)
                    {
                        if (entityNewComp.Attributes[key] != null)
                        {
                            Type type = (entityNewComp.Attributes[key]).GetType();

                            if (type.Name == "String")
                            {
                                var camp1 = entityNewComp.GetAttributeValue<string>(key);
                                var camp2 = entityOrgComp.GetAttributeValue<string>(key);
                                camp1 = camp1 == null ? string.Empty : camp1;
                                camp2 = camp2 == null ? string.Empty : camp2;
                                if (camp1 != camp2)
                                {
                                    sonIguales = false;
                                    break;
                                }
                            }
                            else if (type.Name == "Money")
                            {
                                var camp1 = entityNewComp.GetAttributeValue<Money>(key).Value;
                                var camp2 = entityOrgComp.GetAttributeValue<Money>(key).Value;
                                if (camp1 != camp2)
                                {
                                    sonIguales = false;
                                    break;
                                }
                            }
                            else if (type.Name == "DateTime")
                            {
                                var camp1 = entityNewComp.GetAttributeValue<DateTime>(key);
                                var camp2 = entityOrgComp.GetAttributeValue<DateTime>(key);
                                if (camp1.ToShortDateString() != camp2.ToShortDateString())
                                {
                                    sonIguales = false;
                                    break;
                                }
                            }
                            else if (type.Name == "EntityReference")
                            {
                                var camp1 = entityNewComp.GetAttributeValue<EntityReference>(key).Id;
                                var camp2 = entityOrgComp.GetAttributeValue<EntityReference>(key).Id;
                                if (camp1 != camp2)
                                {
                                    sonIguales = false;
                                    break;
                                }
                            }
                            else if (type.Name == "Boolean")
                            {
                                var camp1 = entityNewComp.GetAttributeValue<Boolean>(key);
                                var camp2 = entityOrgComp.GetAttributeValue<Boolean>(key);
                                if (camp1 != camp2)
                                {
                                    sonIguales = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (entityOrgComp.Attributes.Contains(key))
                            {
                                if (entityOrgComp.Attributes[key] != null)
                                {
                                    var valor = entityOrgComp.Attributes[key];

                                    if (valor != null)
                                    {
                                        sonIguales = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    sonIguales = false;
                }
            }
            else
            {
                sonIguales = false;
            }



            return sonIguales; //(valor1 == valor2);

        }


    }
}
