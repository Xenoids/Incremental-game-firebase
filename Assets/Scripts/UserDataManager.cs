using UnityEngine;
using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress =  new UserProgressData();

    public static IEnumerator LoadFromCloud(System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();
        bool isCompleted = false;
        bool isSuccessfull = false;

        const long maxAllowedSize = 1024 * 1024; // 1MB
        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith ((Task<byte[]> task) =>

        {

            if (!task.IsFaulted)

            {

                string json = Encoding.Default.GetString (task.Result);

                Progress = JsonUtility.FromJson<UserProgressData> (json);

                isSuccessfull = true;

            }

 

            isCompleted = true;

        });

        while(!isCompleted) yield return null;

        // sukses download, simpan datanya
        if(isSuccessfull) Save();

        else LoadFromLocal();

        onComplete?.Invoke();
    }


    public static void LoadFromLocal()
    {
        // Cek apakah ada data tersimpan sbg PROGRESS_KEY
        if(!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // kalo ga ada , buat data baru dan upload ke Cloud
            //Progress = new UserProgressData();
            Save(true);
        }
        else{
            // jika ada, maka overwrite data yang sblmnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData> (json);
        }
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY,json);
         if (uploadToCloud)
        {

        AnalyticsManager.SetUserProperties("gold",Progress.Gold.ToString());
        byte[] data = Encoding.Default.GetBytes (json);

        StorageReference targetStorage = GetTargetCloudStorage ();

 

        targetStorage.PutBytesAsync (data);

        }
    }

    private static StorageReference GetTargetCloudStorage()
    {
        // gunakan id device sbg nama file disimpan di cloud
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl ($"{storage.RootReference}/{deviceID}");
    }

    public static bool HasResources (int index)
    {
        return index + 1 <= Progress.ResLevels.Count;
    }

}
