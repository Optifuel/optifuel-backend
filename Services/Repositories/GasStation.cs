using ApiCos.Data;
using ApiCos.Services.Repositories;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ApiCos.Models.Entities;
using ApiCos.DTOs.GasStationDTO;
using ApiCos.Services.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace ApiCos.Services
{
    public class GasStation : IGasStation
    {
        private readonly ApiDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GasStation(ApiDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<string> UpdateGasStation()
        {
            UpdateGasRegistryTable();
            Console.WriteLine("Gas Station Registry Update Success");
            UpdateGasPriceTable();
            Console.WriteLine("Gas Station Price Update Success");
            return Task.FromResult("Completed");

        }

        public void UpdateGasStationPrice()
        {
            throw new System.NotImplementedException();
        }

        private void UpdateGasRegistryTable()
        {
            int count = 0;
            HttpClient client = new HttpClient();
            var responseTask = Task.Run(() => client.GetAsync("https://www.mimit.gov.it/images/exportCSV/anagrafica_impianti_attivi.csv"));
            responseTask.Wait();
            var response = responseTask.Result;

            var streamTask = Task.Run(() => response.Content.ReadAsStreamAsync());
            streamTask.Wait();

            var tableList = _context.GasStationRegistry.ToList();
            using(var stream = streamTask.Result)
            using(var reader = new StreamReader(stream))
            using(var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null }))
            {
                csv.Read();
                csv.Read();
                while(csv.Read())
                {
                    GasStationRegistry table = new GasStationRegistry();
                    var data = csv.GetRecord<GasStationRegistryRequest>();

                    table = tableList.Find(x => x.Id.Equals(data.idImpianto));
                    if(table != null)
                        continue;

                    if(data == null)
                        continue;

                    table = new GasStationRegistry
                    {
                        Id = data.idImpianto,
                        GasProvider = data.Bandiera,
                        CompanyName = data.Gestore,
                        GasStationName = data.NomeImpianto,
                        Address = data.Indirizzo,
                        City = data.Comune,
                        Province = data.Provincia,
                        Latitude = 0,
                        Longitude = 0
                    };

                    try
                    {
                        string dataString = data.Latitudine.Replace(".", string.Empty);
                        table.Latitude = double.Parse(dataString) / (Math.Pow(10, dataString.Length - 2));

                    } catch(FormatException)
                    {
                        table.Latitude = 0;
                    }

                    try
                    {
                        string dataString = data.Longitudine.Replace(".", string.Empty);
                        table.Longitude = double.Parse(dataString) / (Math.Pow(10, dataString.Length - 2));
                        if(table.Longitude >= 19)
                            table.Longitude = table.Longitude / 10;

                    } catch(FormatException)
                    {
                        table.Longitude = 0;
                    }

                    _context.GasStationRegistry.Add(table);

                }
            }
            _context.SaveChanges();
        }

        private void UpdateGasPriceTable()
        {
            HttpClient client = new HttpClient();
            var responseTask = Task.Run(() => client.GetAsync("https://www.mimit.gov.it/images/exportCSV/prezzo_alle_8.csv"));
            responseTask.Wait();
            var response = responseTask.Result;

            var gasStationRegistryList = _context.GasStationRegistry.ToList();
            var tableList = _context.GasStationPrice.ToList();

            var streamTask = Task.Run(() => response.Content.ReadAsStreamAsync());
            streamTask.Wait();
            using(var stream = streamTask.Result)
            using(var reader = new StreamReader(stream))
            using(var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null }))
            {
                csv.Read();
                csv.Read();
                while(csv.Read())
                {
                    bool newValue = true;
                    GasStationPrice table = new GasStationPrice();
                    var data = csv.GetRecord<GasStationPriceRequest>();
                    if(data == null)
                        continue;

                    var gasStationRegistryId = data.idImpianto;
                    var gasStationRegistry = gasStationRegistryList.Find(x => x.Id.Equals(gasStationRegistryId));
                    if(gasStationRegistry == null)
                    {
                        continue;
                    }
                    if(data.descCarburante.ToLower().Replace(" ", "").Equals("gasolio"))
                        data.descCarburante = "Diesel";

                    table = tableList.Where(x => x.Id.Equals(data.idImpianto) && x.FuelType.Equals(data.descCarburante) && x.IsSelf.Equals(data.isSelf)).FirstOrDefault();
                    if(table != null)
                    {
                        newValue = false;

                        table.LastUpdate = data.dtComu;
                        table.Price = (double)data.prezzo;
                    }
                    else
                    {
                         

                        table = new GasStationPrice()
                        {
                            Id = data.idImpianto,
                            FuelType = data.descCarburante,
                            Price = (double)data.prezzo,
                            LastUpdate = data.dtComu,
                            IsSelf = data.isSelf,
                            GasStationRegistry = gasStationRegistry
                        };
                    }


                    if(newValue)
                        _context.GasStationPrice.Add(table);
                    else 
                        _context.GasStationPrice.Update(table);
                }
            }
            _context.SaveChanges();
        }
    }
}
