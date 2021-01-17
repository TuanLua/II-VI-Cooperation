using System.Collections.Generic;
using System.Linq;
using II_VI_Incorporated_SCM.Models;
using System;
using System.Data.Entity.Validation;

namespace II_VI_Incorporated_SCM.Services
{
    public interface ICCNService
    {
        List<CCN> GetListCCN();
        void CreateNCR_DET(NCR_DET model);
        void CreateNCR_DIS(NCR_DIS model);
        void CreateNCR_HDR(NCR_HDR model);
        void CreateNCR_EVI(NCR_EVI model);
        void UpdateReciever_Defect(string reciever, string ncrnum);
        string GetRecLineByReciever(string reciever);
        string GetSEQInInsResultFinal(string reciever);
        void CreateINS_RESULT_FINAL(INS_RESULT_FINAL model);
        bool CheckReceiverForIQC(NCR_HDR model);
    }
    public class CCNService : ICCNService
    {
        private IIVILocalDB _db;

        public CCNService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public List<CCN> GetListCCN()
        {
            var obj = _db.CCNs.ToList();
            return obj;
        }

        public void CreateNCR_DET(NCR_DET model)
        {
            _db.NCR_DET.Add(model);
            _db.SaveChanges();
        }

        public void CreateINS_RESULT_FINAL(INS_RESULT_FINAL model)
        {
            _db.INS_RESULT_FINAL.Add(model);
            _db.SaveChanges();
        }

        public void UpdateReciever_Defect(string reciever, string ncrnum)
        {
            var list = _db.INS_RESULT_DEFECT.Where(x => x.NCR_Num == null && x.receiver.Trim() == reciever.Trim()).ToList();
            
            foreach(var item in list)
            {
                _db.INS_RESULT_DEFECT.Attach(item);
                item.NCR_Num = ncrnum;
                _db.SaveChanges();
            }
        }

        public string GetRecLineByReciever(string reciever)
        {
            var data = _db.RECEIVERs.Where(x => x.RECEIVER1.Trim() == reciever.Trim()).FirstOrDefault();
            return data != null ? data.REC_LINE : "";
        }

        public string GetSEQInInsResultFinal(string reciever)
        {
            string result = "";
            int tmp = 0;

            var list = _db.INS_RESULT_FINAL.Where(x => x.RECEIVER.Trim() == reciever.Trim()).ToList();
            foreach(var item in list)
            {
                if (Convert.ToInt32(item.SEQ) >= tmp)
                {
                    tmp = Convert.ToInt32(item.SEQ);
                }
            }
            result = (tmp + 1).ToString().Length == 1 ? "0" + (tmp + 1) : (tmp + 1).ToString();
            
            return result;
        }

        public void CreateNCR_DIS(NCR_DIS model)
        {
            _db.NCR_DIS.Add(model);
            _db.SaveChanges();
        }

        public void CreateNCR_HDR(NCR_HDR model)
        {
            try
            {
                _db.NCR_HDR.Add(model);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                var _log = new LogWriter("CreateNCR_HDR");
                _log.LogWrite(ex.ToString());
                if (ex is DbEntityValidationException)
                {
                    var e = (DbEntityValidationException)ex;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        _log.LogWrite(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            _log.LogWrite(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
            }
            //return model.NCR_NUM;
        }

        public void CreateNCR_EVI(NCR_EVI model)
        {
            _db.NCR_EVI.Add(model);
            _db.SaveChanges();
        }

        public bool CheckReceiverForIQC(NCR_HDR model)
        {
            return _db.INS_RESULT_DEFECT.FirstOrDefault(x=>x.receiver.Trim() == model.RECEIVER.Trim() & string.IsNullOrEmpty(x.NCR_Num)) != null;
        }
    }
}