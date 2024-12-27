﻿using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using FirstScraper.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace FirstCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MegaBuyController : ControllerBase
    {
        private readonly ILogger<MegaBuyController> _logger;
        public MegaBuyController(ILogger<MegaBuyController> logger)
        {
            _logger = logger;
        }

        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url = "https://www.megabuy.com.au/dell-refurbished-p2422h-238-ips-fhd-monitor-1920x1080-displayport-hdmi-vga-height-tilt-p1322087.html")
        {
            var product = new ProductDetails
            {
                Name = string.Empty,

                mrp = string.Empty,

                Delivery = string.Empty,

                ImageUrl = [],

                description = [],

                productspecification = new Dictionary<string, string>(),
                //display = [],

                //processor = [],

                //datacarrier = [],

                //camera = [],

                //network = [],

                //portsandinterfaces = [],

                //sendingmessages = [],

                //design = [],

                //Efficiency = [],

                //Multimedia = [],

                //Software = [],

                //Weightanddimensions = [],

                //PackageContents = [],

                //AdditionalInformation = [],

                //Insurance = string.Empty,

                //paymentmethod = string.Empty,

                //productdelivery = string.Empty,

                //productservice = string.Empty,

                ////Discription = string.Empty,

                TechnicalData = new Dictionary<string, string>(),

            };

            try
            {

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(response);

                    var productcategory = document.DocumentNode.SelectNodes("//div[contains(@class,\"pageBreadcrumbTrail2\")]//a//span");
                    if (productcategory != null)
                    {
                        product.productcategory = productcategory
                            .Select(row => $"{row.InnerText.Trim()}")
                            .ToArray(); 
                    }

                    var brandNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productManufacturerValue\"]/a");
                    if (brandNode != null)
                    {
                        product.Brandofproduct = brandNode.InnerText.Trim();
                    }

                    var nameNode = document.DocumentNode.SelectSingleNode("//h1[contains(@class,\"pageHeading\")]");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var modelNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productModelValue\"]");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//div[contains(@id,\"productPriceValue\")]//span[2]");
                    if (mrp != null)
                    {
                        product.mrp = mrp.InnerText.Trim();
                    }
                    var Currency = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"col2\")]//span[1]");
                    if (Currency != null)
                    {
                        product.Currency = Currency.GetAttributeValue("", "N/A");
                    }

                    var deliveryNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productDeliveryValue\"]");
                    if (deliveryNode != null)
                    {
                        product.Delivery = deliveryNode.InnerText.Trim();
                    }

                    //var pricediscount = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"sa\")]");
                    //if (pricediscount != null)
                    //{
                    //    product.pricediscount = pricediscount.InnerText.Trim();
                    //}
                    var imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\"productImageBox\")]//img");
                    if (imageNodes != null)
                    {
                        product.ImageUrl = imageNodes
                            .Select(node => node.GetAttributeValue("src", string.Empty))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .ToArray();
                    }


                    //var displayTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-22']//table//tr");
                    //if (displayTableRows != null)
                    //{
                    //    product.display = displayTableRows
                    //        .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //        .ToArray();
                    //}
                    //var rating = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"w_\")]");
                    //if (rating != null)
                    //{
                    //    product.rating = rating.InnerText.Trim();
                    //}
                    
                    //var specificationRow = document.DocumentNode.SelectNodes("//div[contains(@id,\"idTab20\")]//table[contains(@class,\"spec_table\")]//tbody//tr");
                    //if (specificationRow != null)
                    //{
                    //    product.Specification = specificationRow
                    //        .Select(row =>
                    //        {
                    //            var keyNode = row.SelectSingleNode("./td[1]");
                    //            var valueNode = row.SelectSingleNode("./td[2]");

                    //            if (keyNode != null && valueNode != null)
                    //            {
                    //                return $"{keyNode.InnerText.Trim()}: {valueNode.InnerText.Trim()}";
                    //            }
                    //            return null;
                    //        })
                    //        .Where(item => item != null)
                    //        .ToArray(); 
                    //}

                    var technicaldataNodes = document.DocumentNode.SelectNodes("//div[contains(@id,\"idTab20\")]//table[contains(@class,\"spec_table\")]//tbody//tr");
                    if (technicaldataNodes != null)
                    {
                        foreach ( var detailNode in technicaldataNodes)
                        {
                            var keyNode = detailNode.SelectSingleNode("./td[1]");
                            var valueNode = detailNode.SelectSingleNode("./td[2]");

                            if(keyNode != null && valueNode != null)
                            {
                                string techDataName = keyNode.InnerText.Trim();
                                string techDataValue = valueNode.InnerText.Trim();
                                product.productspecification[techDataName]= techDataValue;
                            }
                        }
                    }

                    var descriptionRow = document.DocumentNode.SelectNodes("//div[contains(@class,\"descriptionContainer\")]");
                    if (descriptionRow != null)
                    {
                        product.description = descriptionRow
                            .Select(row => row.InnerText.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                            .ToArray();
                    }

                    //var reviews = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"vg yc hs\")]"); // Selects the first <font> with non-empty text
                    //if (reviews != null)
                    //{
                    //    product.reviews = reviews.InnerText.Trim();

                        
                    //    var processorTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-24']//table//tr");
                    //    if (processorTableRows != null)
                    //    {
                    //        product.processor = processorTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }
                    //    // Data Carrier Section
                    //    var dataCarrierRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-25']//table//tr");
                    //    if (dataCarrierRows != null)
                    //    {
                    //        product.datacarrier = dataCarrierRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts data carrier details into an array
                    //    }

                    //    // Camera Section
                    //    var cameraRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-26']//table//tr");
                    //    if (cameraRows != null)
                    //    {
                    //        product.camera = cameraRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts camera details into an array
                    //    }

                    //    // Network Section
                    //    var networkRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-27']//table//tr");
                    //    if (networkRows != null)
                    //    {
                    //        product.network = networkRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts network details into an array
                    //    }

                    //    // Ports and Interfaces Section
                    //    var portsAndInterfacesRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-2']//table//tr");
                    //    if (portsAndInterfacesRows != null)
                    //    {
                    //        product.portsandinterfaces = portsAndInterfacesRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts ports and interfaces details into an array
                    //    }


                    //    var sendingMessagesTableRows = document.DocumentNode.SelectNodes("//div[@class='oo' and @data-collabs='specification-section-46']//table//tr");
                    //    if (sendingMessagesTableRows != null)
                    //    {
                    //        var sendingMessagesDetails = sendingMessagesTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray();
                    //        //  product.sendingmessages = string.Join(", ", sendingMessagesDetails);
                    //    }

                    //    var designTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-4']//table//tr");
                    //    if (designTableRows != null)
                    //    {
                    //        var designDetails = designTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray();
                    //        // product.design = string.Join(", ", designDetails);
                    //    }

                    //    var efficiencyTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-9']//table//tr");
                    //    if (efficiencyTableRows != null)
                    //    {
                    //        var efficiencyDetails = efficiencyTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray();
                    //        // product.Efficiency = string.Join(", ", efficiencyDetails);
                    //    }

                    //    var multimediaTableRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-30']//table//tr");
                    //    if (multimediaTableRows != null)
                    //    {
                    //        product.Multimedia = multimediaTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts each multimedia detail into an array
                    //    }

                    //    var softwareTableRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-32']//table//tr");
                    //    if (softwareTableRows != null)
                    //    {
                    //        product.Software = softwareTableRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts software details into an array
                    //    }

                    //    // For Weight and Dimensions section
                    //    var weightAndDimensionsRows = document.DocumentNode.SelectNodes(
                    //        "//div[contains(@class,'oo') and @data-collabs='specification-section-6']//table//tr"
                    //    );
                    //    if (weightAndDimensionsRows != null)
                    //    {
                    //        product.Weightanddimensions = weightAndDimensionsRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts weight and dimensions details into an array
                    //    }

                    //    // For Package Contents section
                    //    var packageContentsRows = document.DocumentNode.SelectNodes(
                    //        "//div[contains(@class,'oo') and @data-collabs='specification-section-15']//table//tr");
                    //    if (packageContentsRows != null)
                    //    {
                    //        product.PackageContents = packageContentsRows
                    //            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //            .ToArray(); // Converts package contents details into an array
                    //    }

                    //    // For Additional Information section
                    //    var Insurance = document.DocumentNode.SelectSingleNode("//section[contains(@id,\"section-insurances\")]"); // Selects the first <font> with non-empty text
                    //    if (Insurance != null)
                    //    {
                    //        product.Insurance = Insurance.InnerText.Trim(); // Extract and trim the inner text

                    //    }

                    //    var productinformation = document.DocumentNode.SelectNodes("//div[contains(@class,\"mb-4 mt-7 leading-relaxed product-description-info prose pl-4\")]//p");
                    //    if (productinformation != null)
                    //    {
                    //        product.productinformation = productinformation
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //    var paymentmethod = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui\")]//div[6]//a//span[contains(@class,\"yl\")]"); // Selects the first <font> with non-empty text
                    //    if (paymentmethod != null)
                    //    {
                    //        product.paymentmethod = paymentmethod.InnerText.Trim(); // Extract and trim the inner text
                    //    }


                    //    var productdelivery = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui\")]//div[5]//a//span[contains(@class,\"yl\")]"); // Selects the first <font> with non-empty text
                    //    if (productdelivery != null)
                    //    {
                    //        product.productdelivery = productdelivery.InnerText.Trim(); // Extract and trim the inner text
                    //    }


                    //    var productservice = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui zf\")]//div[contains(@class,\"hz pn\")]"); // Selects the first <font> with non-empty text
                    //    if (productservice != null)
                    //    {
                    //        product.productservice = productservice.InnerText.Trim(); // Extract and trim the inner text
                    //    }

                    //    var productcategory = document.DocumentNode.SelectNodes("//div[contains(@class,\"top-container\")]//ol//li");
                    //    if (productcategory != null)
                    //    {
                    //        product.productcategory = productcategory
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //    var AdditionalInformation = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-7 description_2\")]//p//text()");
                    //    if (AdditionalInformation != null)
                    //    {
                    //        for (int i = 0; i < AdditionalInformation.Count; i += 2)
                    //        {
                    //            // Extracting the techDataName from the current node
                    //            string techDataName = (i < AdditionalInformation.Count) ? AdditionalInformation[i].InnerText.Trim() : string.Empty;

                    //            // Extracting the techDataValue from the next node
                    //            string techDataValue = (i + 1 < AdditionalInformation.Count) ? AdditionalInformation[i + 1].InnerText.Trim() : string.Empty;

                    //            // Only add if both name and value are valid
                    //            if (!string.IsNullOrWhiteSpace(techDataName) && !string.IsNullOrWhiteSpace(techDataValue))
                    //            {
                    //                product.TechnicalData[techDataName] = techDataValue;
                    //            }
                    //        }
                    //    }


                    //    foreach (var entry in product.TechnicalData)
                    //    {
                    //        // Ensure formatting aligns correctly
                    //        Console.WriteLine($"{entry.Key,-25} | {entry.Value}");
                    //    }


                    //    var highlights = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-3 highlights\")]//p");
                    //    if (highlights != null)
                    //    {
                    //        product.highlights = highlights
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //    var scopeofdelivery = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-3 delivery_scope\")]//p");
                    //    if (scopeofdelivery != null)
                    //    {
                    //        product.scopeofdelivery = scopeofdelivery
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //}
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error fetching product details: {e.Message}");
            }

            return Ok(product);
        }

    }
}