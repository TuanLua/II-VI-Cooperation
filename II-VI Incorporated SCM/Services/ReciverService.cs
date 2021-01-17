using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using II_VI_Incorporated_SCM.Models;
using System.Linq;
using System.Web.Helpers;
using II_VI_Incorporated_SCM.Models.NCR;
using Kendo.Mvc.Infrastructure.Implementation;
using II_VI_Incorporated_SCM.Controllers.NCR;
using System;

namespace II_VI_Incorporated_SCM.Services
{

    public interface IReciverService
    {
        ReciverViewmodel GetListReciver(string reciver, string partnum,string CCN);
        ReciverViewmodel GetListReciverByPO(string po, string lot, string partnum,string CCN);
        List<Children> GetDropdownlist(string ccn);
        List<INS_RESULT_DEFECTViewModel> GetInresult(string item);
        List<DescriptionModel> GetDropdownlistDecript1(List<string> id,string ccn);
        List<Children> GetDropdowndecript();
        List<DescriptionModel> GetDropdownlistIQC(List<string> listInput);

        List<DescriptionModel> GetDropdownlistDesForPro(List<string> listIdDes,string CCN);

        List<List<Children>> GetDropdownlistDefectForPro(string ncrnum,string CCN);
        List<DescriptionModel> GetDropdownlistDecriptByIdDefect(List<string> id,string CCN);
        List<INS_RESULT_PARTIAL> GetListPartial(string receiver);
        CHANGED_SPL GetChangedSpl(string reciver);
        C GetCS(string reciver);
        List<INS_RESULT_DEFECT> GetlistDefectsQty(string receiver);
    }

    public class ReciverService : IReciverService
    {
        private IIVILocalDB _db;

        public ReciverService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public List<INS_RESULT_PARTIAL> GetListPartial(string receiver)
        {
            var listPartialID = _db.INS_RESULT_DEFECT.Where(m => m.receiver.Trim() == receiver.Trim()).Select(o => o.PartialID).ToList();
            var listresult = _db.INS_RESULT_PARTIAL.Where(m => listPartialID.Contains(m.PartialID)).ToList();
            return listresult;
        }

        public List<INS_RESULT_DEFECT> GetlistDefectsQty(string receiver)
        {
            var Result = _db.INS_RESULT_DEFECT.Where(x => x.receiver.Trim() == receiver.Trim()).ToList();
            return Result;
        }
        public CHANGED_SPL GetChangedSpl(string reciver)
        {
            var change = _db.CHANGED_SPL.Where(x => x.RECEIVER.Trim() == reciver.Trim()).FirstOrDefault();
            return change;
        }
        public C GetCS(string reciver)
        {
            var cs = _db.CS.Where(x => x.RECEIVER.Trim() == reciver.Trim()).FirstOrDefault();
            return cs;
        }
        public ReciverViewmodel GetListReciver(string reciver, string partnum,string CCN)
        {
            var _RECEIVER = from rec in _db.RECEIVERs
                            join partial in _db.INS_RESULT_PARTIAL on rec.RECEIVER1.Trim() equals partial.receiver.Trim()
                            join inrl in _db.INS_RESULT on rec.RECEIVER1.Trim() equals inrl.RECEIVER.Trim()
                            //Thi.Nguyen 21-Jan-2010: Add join condition: PUR_LOC
                            //join vender in _db.VENDORs on rec.VENDOR.Trim() equals vender.VENDOR1.Trim()
                            join vender in _db.VENDORs on new { p1=rec.VENDOR,p2= rec.PUR_LOC} equals new {p1= vender.VENDOR1, p2=vender.PUR_LOC }
                            join defect in _db.INS_RESULT_DEFECT on rec.RECEIVER1.Trim() equals defect.receiver.Trim()
                            where (rec.RECEIVER1.Trim() == reciver.Trim() && rec.CCN==CCN && rec.ITEM.Trim() == partnum.Trim()) & inrl.STEP.Trim().Contains("Check Drawing Revision") & defect.NCR_Num == null
                            select new ReciverViewmodel
                            {
                                ITEM = rec.ITEM,
                                RECEIVER1 = rec.RECEIVER1,
                                LOT = rec.LOT,
                               // QTY = rec.QTY,
                                PO_NUM = rec.PO_NUM,
                              //  AQL_VISUAL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                              //  SAMPLING_MEASURE = cs.SAMPLING_MEASURE,
                              //  SAMPLING_VISUAL = cs.SAMPLING_VISUAL,
                             //   AQL_MEASURE = cs.AQL_MEASURE,
                             //   REJ_QTY = partial.PartialRej,
                              //  INS_QTY = partial.PartialIns,
                           //     REC_QTY = cs.QTY,
                                INSPECTOR = partial.Inspector,
                                VEN_NAME = vender.VEN_NAM,
                                VENDOR = vender.VENDOR1,
                                VEN_ADD = vender.ADDRESS,
                                DRAW_REV = inrl.DRAW_REV,
                                ITEM_DESC = rec.ITEM_DESC,
                              //  MODEL_NO = inrl.SERIAL_NUM,
                            //    SKIP_LEVEL = cs.SKIP_LEVEL,
                                ZIP = vender.ZIP,
                                STATE = vender.STATE,
                                CTRY = vender.CTRY,
                              //  AQL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                NC_Qty = defect.NC_Qty,
                                NCR_Num = defect.NCR_Num
                                //INSPECTOR = ?? ID_USer
                            };

            return _RECEIVER.FirstOrDefault();
        }
        public ReciverViewmodel GetListReciverByPO(string po, string partnum, string lot,string CCN)
        {
            var _RECEIVER = from rec in _db.RECEIVERs
                            join partial in _db.INS_RESULT_PARTIAL on rec.RECEIVER1.Trim() equals partial.receiver.Trim()
                            join inrl in _db.INS_RESULT on rec.RECEIVER1.Trim() equals inrl.RECEIVER.Trim()
                            join vender in _db.VENDORs on rec.VENDOR.Trim() equals vender.VENDOR1.Trim()
                            join defect in _db.INS_RESULT_DEFECT on rec.RECEIVER1.Trim() equals defect.receiver.Trim()
                            where (rec.PO_NUM.Trim() == po.Trim() && rec.ITEM.Trim() == partnum.Trim() && rec.LOT.Trim() == lot.Trim() && rec.CCN == CCN) && defect.NCR_Num == null 
                            select new ReciverViewmodel
                            {
                                ITEM = rec.ITEM,
                                RECEIVER1 = rec.RECEIVER1,
                                LOT = rec.LOT,
                               // QTY = rec.QTY,
                                PO_NUM = rec.PO_NUM,
                                //  AQL_VISUAL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                //  SAMPLING_MEASURE = cs.SAMPLING_MEASURE,
                                //  SAMPLING_VISUAL = cs.SAMPLING_VISUAL,
                                //   AQL_MEASURE = cs.AQL_MEASURE,
                              //  REJ_QTY = partial.PartialRej,
                               // INS_QTY = partial.PartialIns,
                                //     REC_QTY = cs.QTY,
                                INSPECTOR = partial.Inspector,
                                VEN_NAME = vender.VEN_NAM,
                                VENDOR = vender.VENDOR1,
                                VEN_ADD = vender.ADDRESS,
                                DRAW_REV = inrl.DRAW_REV,
                                ITEM_DESC = rec.ITEM_DESC,
                              //  MODEL_NO = inrl.SERIAL_NUM,
                                //    SKIP_LEVEL = cs.SKIP_LEVEL,
                                ZIP = vender.ZIP,
                                STATE = vender.STATE,
                                CTRY = vender.CTRY,
                                //  AQL = string.IsNullOrEmpty(cs.AQL_VISUAL) ? cs.AQL_VISUAL : change.AQL_VISUAL,
                                NC_Qty = defect.NC_Qty,
                                NCR_Num = defect.NCR_Num
                                //INSPECTOR = ?? ID_USer
                            };
            return _RECEIVER.FirstOrDefault();
        }
        public List<INS_RESULT_DEFECTViewModel> GetInresult(string reciver)
        {
            List<INS_RESULT_DEFECTViewModel> listResult = new List<INS_RESULT_DEFECTViewModel>();
            var listrDets = _db.INS_RESULT_DEFECT.Where(m => m.receiver.Trim() == reciver.Trim()).ToList();
            if (listrDets != null)
            {
                foreach (var item in listrDets)
                {
                    List<string> listdefect = new List<string>();
                    List<string> listNonConform = new List<string>();
                    if (item.Defect != null && item.Defect != "")
                    {
                        listdefect = item.Defect.Split(';').ToList();
                    }
                    if (item.Non_Conformances != null && item.Non_Conformances != "")
                    {
                        listNonConform = item.Non_Conformances.Split(';').ToList();
                    }

                    listResult.Add(new INS_RESULT_DEFECTViewModel
                    {
                        CCN = item.CCN,
                        Defect = listdefect,
                        Disposition = item.Disposition,
                        NCR_Num = item.NCR_Num,
                        NC_Qty = item.NC_Qty,
                        Non_Conformances = listNonConform,
                        PartialID = item.PartialID,
                        Picture = item.Picture,
                        receiver = item.receiver,
                        rec_line = item.rec_line,
                        Remark = item.Remark,
                        Response = item.Response
                    });
                }
            }
            return listResult;
        }
        public List<Children> GetDropdownlist(string ccn)
        {
            var _NC_GROUP = _db.NC_GROUP.Where(x=>x.CCN ==ccn).ToList();
            List<Children> list = new List<Children>();
            foreach (var item in _NC_GROUP)
            {
                list.Add(new Children
                {
                    label = item.NC_GRP_DESC,
                    value = item.NC_GRP_CODE,
                    selected = false,
                   // disabled = true
                });
            }
            return list;
        }
        public List<Children> GetDropdowndecript()
        {
            var _NC = _db.NCs.ToList();
            List<Children> list = new List<Children>();
            foreach (var item in _NC)
            {
                list.Add(new Children
                {
                    label = item.NC_DESC,
                    selected = false,
                    value = item.NC_CODE,
                  //  disabled = true
                });
            }
            return list;
        }
        public string GetListDefect_Process(string num)
        {
            var ncr_det = _db.NCR_DET.Where(x => x.NCR_NUM == num).FirstOrDefault();
            if (ncr_det != null)
            {
                return ncr_det.DEFECT;
            }
            else
            {
                return "";
            }
        }
        public List<Children> GetListDefect_ProcessDDL(string num)
        {
            List<string> listdefect = new List<string>();
            string def = GetListDefect_Process(num);
            if (def != null && def != "")
            {
                listdefect = def.Split(';').ToList();
            }

            var _NC_GROUP = _db.NC_GROUP.ToList();
            List<Children> list = new List<Children>();
            foreach (var item in _NC_GROUP)
            {
                int dem = 0;
                bool select = false;
                foreach (var item2 in listdefect)
                {
                    if (item2 == item.NC_GRP_CODE)
                    {
                        dem++;
                        break;
                    }
                }

                if (dem != 0)
                {
                    select = true;
                }

                list.Add(new Children
                {
                    label = item.NC_GRP_DESC,
                    selected = select,
                    value = item.NC_GRP_CODE
                });
            }
            return list;
        }

        public List<DescriptionModel> GetDropdownlistIQC(List<string> listInput)
        {
            if (listInput!= null)
            {
                List<DescriptionModel> list = new List<DescriptionModel>();
                var _NC = from nc in _db.NCs
                    join ncg in _db.NC_GROUP on nc.NC_GRP_CODE equals ncg.NC_GRP_CODE
                    where (listInput.Contains(nc.NC_GRP_CODE))
                    select new NCViewmodel
                    {
                        NC_GRP_CODE = nc.NC_GRP_CODE,
                        NC_GRP_DESC = ncg.NC_GRP_DESC,
                        NC_CODE = nc.NC_CODE,
                        NC_DESC = nc.NC_DESC
                    };
                var group = _NC.GroupBy(m => m.NC_GRP_CODE).Select(m => new { Key = m.Key, Datas = m.ToList() });
                foreach (var item in group)
                {
                    DescriptionModel defectModel = new DescriptionModel();
                    defectModel.children = new List<Children>();
                    foreach (var data in item.Datas)
                    {
                        //List<string> a = new List<string>();
                        //if (listInput != null)
                        //{
                        //    a = listInput.Where(m => m.Contains(data.NC_GRP_CODE)).ToList();
                        //}

                        //bool select = false;
                        //if (a != null && a.Count != 0)
                        //{
                        //    select = true;
                        //}
                        defectModel.label = data.NC_GRP_DESC;
                        defectModel.children.Add(new Children
                        {
                            label = data.NC_DESC,
                            value = data.NC_CODE,
                            selected = false,
                            //disabled = true,
                        });
                    }
                    list.Add(defectModel);
                }
                return list;
            }
            return new List<DescriptionModel>();

        }

        public List<DescriptionModel> GetDropdownlistDecript1(List<string> id,string ccn)
        {
            List<DescriptionModel> list = new List<DescriptionModel>();
            if (id != null)
            {
                var nct = _db.NC_GROUP.Where(x => id.Contains(x.NC_GRP_CODE) & x.CCN ==ccn).ToList();
                var arr = nct.Select(x => x.NC_GRP_CODE).ToArray();
                var ncs = _db.NCs.Where(x => arr.Contains(x.NC_GRP_CODE) & x.CCN == ccn).ToList();

                var _NC = from nc in ncs
                          join ncg in nct on nc.NC_GRP_CODE equals ncg.NC_GRP_CODE
                             where (id.Contains(nc.NC_GRP_CODE) & nc.CCN == ncg.CCN)
                          select new NCViewmodel
                          {
                              NC_GRP_CODE = nc.NC_GRP_CODE,
                              NC_GRP_DESC = ncg.NC_GRP_DESC,
                              NC_CODE = nc.NC_CODE,
                              NC_DESC = nc.NC_DESC
                          };
                //_NC = _NC.Where(m => id.Contains(m.NC_GRP_CODE));
                var group = _NC.GroupBy(m => m.NC_GRP_DESC).Select(m => new { Key = m.Key, Datas = m.ToList() });
                foreach (var item in group)
                {
                    DescriptionModel defectModel = new DescriptionModel();
                    defectModel.label = item.Key;
                    defectModel.children = new List<Children>();
                    foreach (var data in item.Datas)
                    {
                        defectModel.children.Add(new Children
                        {
                            label ="(" + data.NC_CODE +")" + " + " + data.NC_DESC,
                            value = data.NC_CODE,
                            selected = false
                        });
                    }
                    list.Add(defectModel);
                }
                return list;
            }
            else
            {
                return new List<DescriptionModel>();
            }
        }

        public List<DescriptionModel> GetDropdownlistDesForPro(List<string> listIdDes,string CCN)
        {
            List<DescriptionModel> list = new List<DescriptionModel>();
            var _NC = from nc in _db.NCs
                      join ncg in _db.NC_GROUP on nc.NC_GRP_CODE equals ncg.NC_GRP_CODE
                       where (nc.CCN == ncg.CCN)
                      select new NCViewmodel
                      {
                          NC_GRP_CODE = nc.NC_GRP_CODE,
                          NC_GRP_DESC = ncg.NC_GRP_DESC,
                          NC_CODE = nc.NC_CODE,
                          NC_DESC = nc.NC_DESC,
                          CCN = nc.CCN
                      };
            var group = _NC.Where(x=>x.CCN == CCN).GroupBy(m => m.NC_GRP_DESC).Select(m => new { Key = m.Key, Datas = m.ToList() });
            foreach (var item in group)
            {
                DescriptionModel defectModel = new DescriptionModel();
                defectModel.label = item.Key;
                defectModel.children = new List<Children>();
                foreach (var data in item.Datas)
                {
                    List<string> a = new List<string>();
                    if (listIdDes != null)
                    {
                        a = listIdDes.Where(m => m.Contains(data.NC_CODE)).ToList();
                    }

                    bool select = false;
                    if (a != null && a.Count != 0)
                    {
                        select = true;
                    }
                    defectModel.children.Add(new Children
                    {
                        label = data.NC_DESC,
                        title = data.NC_GRP_DESC,
                        value = data.NC_CODE,
                        selected = select
                    });
                }
                list.Add(defectModel);
            }
            return list;
        }

        public List<List<Children>> GetDropdownlistDefectForPro(string ncrnum,string CCN)
        {
            List<List<Children>> listResults = new List<List<Children>>();
            var _NC_GROUP = _db.NC_GROUP.Where(x=>x.CCN ==CCN).ToList();
            var lstDet = _db.NCR_DET.Where(m => m.NCR_NUM == ncrnum).ToList();
            List<Children> list = new List<Children>();
            foreach (var item in _NC_GROUP)
            {
                list.Add(new Children
                {
                    label = item.NC_GRP_DESC,
                    title = item.NC_GRP_DESC,
                    selected = false,
                    value = item.NC_GRP_CODE
                });
            }

            for (var k = 0; k < lstDet.Count; k++)
            {
                var tmp = lstDet[k];
                List<string> tmp_Defect = tmp.DEFECT.Split(';').ToList();
                tmp_Defect = TrimAll(tmp_Defect);
                var lstTmp = new List<Children>();
                foreach (var dt in list)
                {
                    if (tmp_Defect.IndexOf(dt.value) >= 0)
                    {
                        var model = new Children
                        {
                            selected = true,
                            value = dt.value,
                            label = dt.label
                        };

                        lstTmp.Add(model);
                    }
                    else
                    {
                        var model = new Children
                        {
                            selected = false,
                            value = dt.value,
                            label = dt.label
                        };

                        lstTmp.Add(model);
                    }
                }
                listResults.Add(lstTmp);
            }
            return listResults;
        }

        private List<string> TrimAll(List<string> lst)
        {
            for (var i = 0; i < lst.Count; i++)
            {

                lst[i] = lst[i].Trim();
            }

            return lst;
        }

        public List<DescriptionModel> GetDropdownlistDecriptByIdDefect(List<string> id,string CCN)
        {
            List<DescriptionModel> list = new List<DescriptionModel>();
            if (id != null)
            {
                var _NC = from nc in _db.NCs
                          join ncg in _db.NC_GROUP on nc.NC_GRP_CODE equals ncg.NC_GRP_CODE
                          where(nc.CCN == ncg.CCN)
                          select new NCViewmodel
                          {
                              NC_GRP_CODE = nc.NC_GRP_CODE,
                              NC_GRP_DESC = ncg.NC_GRP_DESC,
                              NC_CODE = nc.NC_CODE,
                              NC_DESC = nc.NC_DESC,
                              CCN = nc.CCN
                          };
                var group = _NC.Where(x=>x.CCN == CCN).GroupBy(m => m.NC_GRP_CODE).Select(m => new { Key = m.Key, Datas = m.ToList() }).ToList();
                foreach (var item in group)
                {
                    if (id.IndexOf(item.Key) >= 0)
                    {
                        DescriptionModel defectModel = new DescriptionModel();
                        defectModel.label = item.Datas[0].NC_GRP_DESC;
                        defectModel.children = new List<Children>();
                        foreach (var data in item.Datas)
                        {
                            defectModel.children.Add(new Children
                            {
                                label = "(" + data.NC_CODE + ")" + " + " + data.NC_DESC,
                                value = data.NC_CODE,
                                selected = false
                            });
                        }
                        list.Add(defectModel);
                    }
                }
                return list;
            }
            else
            {
                return new List<DescriptionModel>();
            }
        }
    }
}