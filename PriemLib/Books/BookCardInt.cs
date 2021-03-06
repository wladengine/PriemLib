﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class BookCardInt : BookCard
    {
        public BookCardInt()
        {
            InitializeComponent();
            _Id = null;
        }

        public BookCardInt(string id)
        {
            InitializeComponent();
            _Id = id;
        }

        protected int? IntId
        {
            get
            {
                if (_Id == null)
                    return null;
                else
                    return int.Parse(_Id);
            }
        }

        protected override string Save()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id == null)
                    {
                        ObjectParameter entId = new ObjectParameter("id", typeof(int));
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                InsertRec(context, entId);
                                SaveManyToMany(context, (int)entId.Value);

                                transaction.Complete();
                            }
                            catch (Exception exc)
                            {
                                throw exc;
                            }
                        }
                        return entId.Value.ToString();
                    }
                    else
                    {
                        int entId = IntId.Value;
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                UpdateRec(context, entId);
                                SaveManyToMany(context, entId);

                                transaction.Complete();
                            }
                            catch (Exception exc)
                            {
                                throw exc;
                            }
                        }
                        return entId.ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        protected virtual void SaveManyToMany(PriemEntities context, int id)
        {
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            throw new NotImplementedException("Метод не реализован");
        }

        protected virtual void UpdateRec(PriemEntities context, int id)
        {
            throw new NotImplementedException("Метод не реализован");
        }
    }
}
