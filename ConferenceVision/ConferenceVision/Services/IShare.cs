using System;
using System.Threading.Tasks;

namespace ConferenceVision.Services
{
	public interface IShare
    {
        Task Show(string title, string message, string filePath);
    }
}
