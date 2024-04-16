﻿using BetaCinema.DAO;
using BetaCinema.DTO;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaCinema.GUI.Employee
{
    public partial class fEProduct : Form
    {
        public fEProduct()
        {
            InitializeComponent();

            LoadProduct();
            CustomizeDataGridView();
        }

        #region Methods
        void LoadProduct()
        {
            cboProduct.Items.Clear();
            List<ProductDTO> list = ProductDAO.Instance.GetListProduct();
            cboProduct.DataSource = list;
            cboProduct.DisplayMember = "TenSP";
        }

        private void CustomizeDataGridView()
        {
            AddColumnDataGridView("TenSP", "Sản phẩm");
            AddColumnDataGridView("GiaBan", "Đơn giá");
            AddColumnDataGridView("SoLuong", "Số lượng");
            AddColumnDataGridView("ThanhTien", "Thành tiền");

            DataGridViewImageColumn deleteImageColumn = new DataGridViewImageColumn();
            deleteImageColumn.Name = "DeleteColumn";
            deleteImageColumn.HeaderText = "";
            deleteImageColumn.Image = Properties.Resources.delete;
            dgvProduct.Columns.Add(deleteImageColumn);

            dgvProduct.EnableHeadersVisualStyles = false;
            dgvProduct.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(1, 81, 152);
            dgvProduct.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProduct.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvProduct.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProduct.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dgvProduct.RowTemplate.Height = 30;

            dgvProduct.Columns["TenSP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvProduct.Columns["GiaBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProduct.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProduct.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        
            int totalWidth = dgvProduct.Width;
            dgvProduct.Columns["TenSP"].Width = (int)(0.3 * totalWidth);
            dgvProduct.Columns["GiaBan"].Width = (int)(0.2 * totalWidth);
            dgvProduct.Columns["SoLuong"].Width = (int)(0.2 * totalWidth);
            dgvProduct.Columns["ThanhTien"].Width = (int)(0.25 * totalWidth);
            dgvProduct.Columns["DeleteColumn"].Width = (int)(0.05 * totalWidth);
            dgvProduct.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void AddColumnDataGridView(string colName, string colHeader)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = colName;
            column.HeaderText = colHeader;
            dgvProduct.Columns.Add(column);
        }

        private bool IsValidQuantity()
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                if (!int.TryParse(txtQuantity.Text, out int soLuong) || soLuong > 0)
                {
                    if (soLuong <= int.Parse(txtQuantityStock.Text))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Số lượng nhập vào phải nhỏ hơn hoặc bằng số lượng tồn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtQuantity.Focus();
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Số lượng phải là số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtQuantity.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số lượng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQuantity.Focus();
                return false;
            }
        }

        private void CalculateTotalPrice()
        {
            double tongTien = 0;
            foreach (DataGridViewRow row in dgvProduct.Rows)
            {
                if (!row.IsNewRow)
                {
                    double thanhTien = Convert.ToDouble(row.Cells["ThanhTien"].Value);
                    tongTien += thanhTien;
                }
            }

            txtTotal.Text = string.Format("{0:N0} đ", tongTien);
        }

        #endregion

        #region Events
        private void cboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProductDTO prod = (ProductDTO)cboProduct.SelectedItem;
            string productID = prod.MaSP;

            List<ProductDTO> productList = ProductDAO.Instance.GetListProductByProductID(productID);
            if (productList != null && productList.Count > 0)
            {
                ProductDTO product = productList[0];
                txtQuantityStock.Text = product.SoLuongTon.ToString();
                txtPrice.Text = product.GiaBan.ToString();

                if (product.HinhAnh != null)
                {
                    byte[] posterData = (byte[])product.HinhAnh;
                    using (MemoryStream ms = new MemoryStream(posterData))
                    {
                        picPoster.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    picPoster.Image = Properties.Resources.poster;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!IsValidQuantity())
            {
                return;
            }
            int soLuong = int.Parse(txtQuantity.Text);
            int donGia = int.Parse(txtPrice.Text);
            int thanhTien = int.Parse(txtQuantity.Text) * int.Parse(txtPrice.Text);

            dgvProduct.Rows.Add(cboProduct.Text, soLuong, donGia, thanhTien);
            CalculateTotalPrice();
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvProduct.Columns["DeleteColumn"].Index && e.RowIndex >= 0)
            {
                int rowIndex = e.RowIndex;

                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvProduct.Rows.RemoveAt(rowIndex);
                    CalculateTotalPrice();
                }
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.Hide();
            fBillInfo f = new fBillInfo();
            f.Text = "Thông tin hóa đơn";
            f.ShowDialog();
        }
        #endregion
    }
}
