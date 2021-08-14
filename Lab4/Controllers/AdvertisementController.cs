using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using System.IO;
using Azure;
using Lab4.Models.ViewModels;
using System.Linq;

namespace Lab4.Controllers
{
    public class Advertisements : Controller
    {
        private readonly SchoolCommunityContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "Container";


        // Allows you to manipulate the Azure Storage services and blob containers
        public Advertisements(SchoolCommunityContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        //This provides the content to search sort and manipulate list
        public async Task<IActionResult> Index()
        {
            return View(await _context.Advertisements.ToListAsync());
            //return View();

        }

        //returns the view results when clicked on 
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Upload(IFormFile file)
        {

            BlobContainerClient containerClient;
            // Create the container and return a container client object
            try
            {
                //Creates a new blob under the specified account. If the container with the same name already exist, it will fail.
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);

                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                //A blob client for the desired container is returned
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            }


            try
            {
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(file.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var image = new Advertisement
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = file.FileName
                };

                //Sets the container list to Earth or Computer depending on the the users choice


                _context.Advertisements.Add(image);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                View("Error");
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            // When the id is not found
            if (id == null)
            {
                return NotFound();
            }

            //This will return the first element of a sequence that satisfies a condition 
            // or a default value if no such element is found
            var image = await _context.Advertisements.FirstOrDefaultAsync(m => m.AnswerImageId == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Advertisements.FindAsync(id);


            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                // Get the blob that holds the data
                var blockBlob = containerClient.GetBlobClient(image.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                _context.Advertisements.Remove(image);
                await _context.SaveChangesAsync();

            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            return RedirectToAction("Index");
        }

    }
}
