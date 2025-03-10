﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TicketsHub_for_Desktop
{
    public partial class Lobby : Form
    {
        private TicketDbEntities db;
        private string nm;
        private bool isUpdating = false; // Untuk mencegah event recursive

        public Lobby(string nama)
        {
            InitializeComponent();
            db = new TicketDbEntities();
            nm = nama;
            bigLabel1.Text = $"Halo, {nm}!";
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            LoadGenres(); // Load semua genre
            LoadMovies();  // Load semua film
        }

        private void LoadMovies(string genre = null)
        {
            isUpdating = true;

            var movies = string.IsNullOrEmpty(genre)
                ? db.movies.Select(x => x.Judul).ToList()
                : db.movies.Where(x => x.Genre == genre).Select(x => x.Judul).ToList();

            movies.Insert(0, ""); // Tambahkan opsi None di awal

            cbJudul.DataSource = new List<string>(movies);
            cbJudul.SelectedIndex = 0; // Pilih None sebagai default

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            autoCompleteData.AddRange(movies.ToArray());
            cbJudul.AutoCompleteCustomSource = autoCompleteData;

            isUpdating = false;
        }

        private void LoadGenres(string judul = null)
        {
            isUpdating = true;

            var genres = db.movies.Select(x => x.Genre).Distinct().ToList();
            string selectedGenre = null;

            if (!string.IsNullOrEmpty(judul))
            {
                selectedGenre = db.movies
                    .Where(x => x.Judul == judul)
                    .Select(x => x.Genre)
                    .FirstOrDefault();

                genres = new List<string> { selectedGenre };
            }
            else
            {
                genres.Insert(0, ""); // Tambahkan opsi None di awal
            }

            cbGenre.DataSource = genres;
            cbGenre.SelectedIndex = 0; // Pilih None sebagai default

            isUpdating = false;
        }
        private void cbGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            string selectedGenre = cbGenre.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedGenre))
            {
                LoadMovies(); // Jika genre kosong, tampilkan semua film
            }
            else
            {
                LoadMovies(selectedGenre); // Filter judul berdasarkan genre
            }
        }

        private void cbJudul_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            string selectedJudul = cbJudul.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedJudul))
            {
                LoadGenres(); // Jika judul kosong, tampilkan semua genre
            }
            else
            {
                LoadGenres(selectedJudul); // Tampilkan genre sesuai judul
            }
        }

        private void btnCariFIlm_Click(object sender, EventArgs e)
        {
            string selectedJudul = cbJudul.SelectedItem?.ToString();
            string selectedGenre = cbGenre.SelectedItem?.ToString();

            List_ticket listTicketForm = new List_ticket(selectedJudul, selectedGenre);
            listTicketForm.ShowDialog();
        }
    }
}
