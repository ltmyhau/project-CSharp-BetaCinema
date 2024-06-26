﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaCinema
{
    public partial class fMovieDetail : Form
    {
        string trailerURL = "";

        public fMovieDetail()
        {
            InitializeComponent();
        }

        #region Methods
        public void LoadData(DataGridViewRow selectedRow)
        {
            lblMovieID.Text = selectedRow.Cells["MaPhim"].Value?.ToString();
            txtMovieName.Text = selectedRow.Cells["TenPhim"].Value?.ToString();
            txtCountry.Text = selectedRow.Cells["QuocGia"].Value?.ToString();
            txtDuration.Text = selectedRow.Cells["ThoiLuong"].Value?.ToString();
            txtDirector.Text = selectedRow.Cells["DaoDien"].Value?.ToString();
            txtGenre.Text = selectedRow.Cells["TheLoaiPhim"].Value?.ToString();
            txtSynopsis.Text = selectedRow.Cells["MoTa"].Value?.ToString();
            txtReleaseDate.Text = DateTime.Parse(selectedRow.Cells["NgayKhoiChieu"].Value?.ToString()).ToString("dd/MM/yyyy");
            trailerURL = selectedRow.Cells["Trailer"].Value?.ToString();

            if (selectedRow.Cells["BieuTuongPL"].Value != null)
            {
                byte[] bieuTuongPL = (byte[])selectedRow.Cells["BieuTuongPL"].Value;
                using (MemoryStream ms = new MemoryStream(bieuTuongPL))
                {
                    picRated.Image = Image.FromStream(ms);
                }
            }
            else
            {
                picRated.Image = null;
            }

            if (selectedRow.Cells["Poster"].Value != null)
            {
                byte[] posterData = (byte[])selectedRow.Cells["Poster"].Value;
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
        #endregion

        #region Events
        private void fMovieDetail_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(trailerURL))
            {
                btnTrailer.Visible = false;
            }
            else
            {
                btnTrailer.Enabled = true;
            }
        }

        private void btnTrailer_Click(object sender, EventArgs e)
        {
            fMovieTrailer fMovieTrailer = new fMovieTrailer(trailerURL);
            fMovieTrailer.ShowDialog();
        }

        private void btnTrailer_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void btnTrailer_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }
        #endregion
    }
}
