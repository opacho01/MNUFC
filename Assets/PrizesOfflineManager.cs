using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PrizeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] PrizeObjects = new GameObject[30];
    public Button CloseButton;
    public Button SaveButton;
    public Button LoadButton;

    [Header("Offline Prizes Management")]
    public TMP_InputField NameToAdd;
    public Button AddButton;
    public TMP_Dropdown NameToErase;
    public Button EraseButton;

    [Header("Storage Keys")]
    public string closeStorageKey = "PrizeCloseData";
    public string saveStorageKey = "PrizeSaveData";
    public string offlinePrizesKey = "OfflinePrizes";

    private List<PrizeObjectUI> prizeUIs = new List<PrizeObjectUI>();
    private List<string> prizeNames = new List<string>();
    private const int TOTAL_PRIZES = 30;
    private const int ROWS = 5;
    private const int COLUMNS = 6;

    [System.Serializable]
    public class PrizeObjectUI
    {
        public GameObject gameObject;
        public TMP_Text slotIDText;
        public TMP_Dropdown nameToShowDropdown;
        public TMP_InputField probabilityInput;
        public Toggle inStockToggle;
        public TMP_Dropdown quantityDropdown;
        public string slotID;
    }

    [System.Serializable]
    public class PrizeCollection
    {
        public List<Prize> prizes = new List<Prize>();
    }

    void OnEnable()
    {
        InitializeOfflinePrizes();
        InitializePrizeObjects();
        SetupButtonListeners();
        LoadCloseData();
    }

    /// <summary>
    /// Initializes offline prizes list and UI
    /// </summary>
    private void InitializeOfflinePrizes()
    {
        LoadPrizeNames();

        AddButton.onClick.AddListener(AddPrize);
        EraseButton.onClick.AddListener(EraseSelectedPrize);

        UpdateEraseDropdown();
    }

    /// <summary>
    /// Initializes all prize objects and their UI components
    /// </summary>
    private void InitializePrizeObjects()
    {
        prizeUIs.Clear();

        for (int i = 0; i < TOTAL_PRIZES; i++)
        {
            if (PrizeObjects[i] == null)
            {
                Debug.LogError($"PrizeObject {i} is not assigned in the inspector");
                continue;
            }

            string slotID = $"s{i}";
            GameObject prizeObj = PrizeObjects[i];

            PrizeObjectUI prizeUI = new PrizeObjectUI
            {
                gameObject = prizeObj,
                slotIDText = prizeObj.transform.Find("SlotID")?.GetComponent<TMP_Text>(),
                nameToShowDropdown = prizeObj.transform.Find("NameToShow")?.GetComponent<TMP_Dropdown>(),
                probabilityInput = prizeObj.transform.Find("Probability")?.GetComponent<TMP_InputField>(),
                inStockToggle = prizeObj.transform.Find("InStock")?.GetComponent<Toggle>(),
                quantityDropdown = prizeObj.transform.Find("Quantity")?.GetComponent<TMP_Dropdown>(),
                slotID = slotID
            };

            // Set slot ID text
            if (prizeUI.slotIDText != null)
            {
                prizeUI.slotIDText.text = slotID;
            }

            // Initialize nameToShow dropdown with prize names
            if (prizeUI.nameToShowDropdown != null)
            {
                UpdatePrizeDropdown(prizeUI.nameToShowDropdown);
            }

            // Initialize quantity dropdown (0-10)
            if (prizeUI.quantityDropdown != null)
            {
                InitializeQuantityDropdown(prizeUI.quantityDropdown);
            }

            prizeUIs.Add(prizeUI);
        }
    }

    /// <summary>
    /// Initializes quantity dropdown with values 0-10
    /// </summary>
    /// <param name="dropdown">Dropdown to initialize</param>
    private void InitializeQuantityDropdown(TMP_Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> quantityOptions = new List<string>();
        for (int q = 0; q <= 10; q++)
        {
            quantityOptions.Add(q.ToString());
        }
        dropdown.AddOptions(quantityOptions);
    }

    /// <summary>
    /// Sets up button click listeners
    /// </summary>
    private void SetupButtonListeners()
    {
        CloseButton.onClick.AddListener(OnCloseButtonClicked);
        SaveButton.onClick.AddListener(OnSaveButtonClicked);
        LoadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    /// <summary>
    /// Converts slot ID to row and column name format
    /// </summary>
    /// <param name="slotID">Slot ID in format s0-s29</param>
    /// <returns>Name in format rXcY</returns>
    private string SlotIDToName(string slotID)
    {
        if (string.IsNullOrEmpty(slotID) || !slotID.StartsWith("s"))
            return "r0c0";

        if (int.TryParse(slotID.Substring(1), out int slotNumber))
        {
            if (slotNumber >= 0 && slotNumber < TOTAL_PRIZES)
            {
                int row = slotNumber / COLUMNS;
                int column = slotNumber % COLUMNS;
                return $"r{row}c{column}";
            }
        }

        return "r0c0";
    }

    /// <summary>
    /// Adds a new prize to the offline names list
    /// </summary>
    private void AddPrize()
    {
        string newPrize = NameToAdd.text.Trim();

        if (string.IsNullOrEmpty(newPrize))
        {
            Debug.LogWarning("Cannot add an empty prize");
            return;
        }

        if (prizeNames.Contains(newPrize))
        {
            Debug.LogWarning($"Prize '{newPrize}' already exists in the list");
            return;
        }

        prizeNames.Add(newPrize);
        NameToAdd.text = "";

        SavePrizeNames();
        UpdateEraseDropdown();
        UpdateAllPrizeDropdowns();

        Debug.Log($"Prize added: {newPrize}");
    }

    /// <summary>
    /// Deletes the prize selected in the erase dropdown
    /// </summary>
    private void EraseSelectedPrize()
    {
        if (NameToErase.options.Count == 0 || NameToErase.value == 0)
        {
            Debug.LogWarning("No prizes to delete or none selected");
            return;
        }

        string prizeToErase = NameToErase.options[NameToErase.value].text;

        if (prizeNames.Contains(prizeToErase))
        {
            prizeNames.Remove(prizeToErase);
            SavePrizeNames();
            UpdateEraseDropdown();
            UpdateAllPrizeDropdowns();

            Debug.Log($"Prize deleted: {prizeToErase}");
        }
    }

    /// <summary>
    /// Updates the erase dropdown with current prize names
    /// </summary>
    private void UpdateEraseDropdown()
    {
        NameToErase.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("Select prize to delete..."));

        foreach (string prize in prizeNames)
        {
            options.Add(new TMP_Dropdown.OptionData(prize));
        }

        NameToErase.AddOptions(options);
        NameToErase.value = 0;
    }

    /// <summary>
    /// Updates all prize dropdowns in the scene
    /// </summary>
    private void UpdateAllPrizeDropdowns()
    {
        foreach (var prizeUI in prizeUIs)
        {
            if (prizeUI.nameToShowDropdown != null)
            {
                UpdatePrizeDropdown(prizeUI.nameToShowDropdown);
            }
        }
    }

    /// <summary>
    /// Updates an individual prize dropdown
    /// </summary>
    /// <param name="dropdown">Dropdown to update</param>
    private void UpdatePrizeDropdown(TMP_Dropdown dropdown)
    {
        string currentSelection = dropdown.value > 0 && dropdown.value < dropdown.options.Count ?
                                dropdown.options[dropdown.value].text : "";

        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("-- Select Prize --"));

        foreach (string prize in prizeNames)
        {
            options.Add(new TMP_Dropdown.OptionData(prize));
        }

        dropdown.AddOptions(options);

        int newIndex = prizeNames.IndexOf(currentSelection);
        if (newIndex >= 0)
        {
            dropdown.value = newIndex + 1;
        }
        else
        {
            dropdown.value = 0;
        }

        dropdown.RefreshShownValue();
    }

    /// <summary>
    /// Saves prize names to PlayerPrefs
    /// </summary>
    private void SavePrizeNames()
    {
        string prizesString = string.Join("|", prizeNames.ToArray());
        PlayerPrefs.SetString(offlinePrizesKey, prizesString);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads prize names from PlayerPrefs
    /// </summary>
    private void LoadPrizeNames()
    {
        prizeNames.Clear();

        if (PlayerPrefs.HasKey(offlinePrizesKey))
        {
            string prizesString = PlayerPrefs.GetString(offlinePrizesKey);
            if (!string.IsNullOrEmpty(prizesString))
            {
                string[] prizesArray = prizesString.Split('|');
                prizeNames.AddRange(prizesArray.Where(prize => !string.IsNullOrEmpty(prize)));
            }
        }

        Debug.Log($"Loaded {prizeNames.Count} prize names");
    }

    /// <summary>
    /// Handles Close button click - validates and saves data
    /// </summary>
    private void OnCloseButtonClicked()
    {
        if (ValidateAllPrizeData())
        {
            SaveCloseData();
            panelPrizesSettings.SetActive(false);
            Debug.Log("Close data saved successfully");
        }
    }

    /// <summary>
    /// Handles Save button click - saves current state
    /// </summary>
    private void OnSaveButtonClicked()
    {
        SaveCurrentState();
        Debug.Log("Current state saved successfully");
    }

    /// <summary>
    /// Handles Load button click - loads saved state
    /// </summary>
    private void OnLoadButtonClicked()
    {
        LoadSavedState();
        Debug.Log("Saved state loaded successfully");
    }

    private string showError = "";
    public TMP_Text TextErrorPrizes;
    public GameObject panelErrorPrizes;
    public GameObject panelPrizesSettings;

    /// <summary>
    /// Validates all prize objects for required data
    /// </summary>
    /// <returns>True if all data is valid</returns>
    public bool ValidateAllPrizeData()
    {
        bool allValid = true;
        showError = "";
        foreach (var prizeUI in prizeUIs)
        {
            if (!ValidatePrizeObject(prizeUI))
            {
                allValid = false;
                showError += "Missing data in object with Slot ID: " + prizeUI.slotID + "\n";
            }
        }

        if (!allValid)
        {
            Debug.LogError("Please complete all required fields in all prize objects");
            TextErrorPrizes.text = showError;
            panelErrorPrizes.SetActive(true);
        } else
        {
            
        }

        return allValid;
    }

    /// <summary>
    /// Validates individual prize object data
    /// </summary>
    /// <param name="prizeUI">Prize object to validate</param>
    /// <returns>True if data is valid</returns>
    private bool ValidatePrizeObject(PrizeObjectUI prizeUI)
    {
        // Check nameToShow dropdown has selection
        if (prizeUI.nameToShowDropdown == null || prizeUI.nameToShowDropdown.value == 0)
            return false;

        // Check probability input has valid value
        if (prizeUI.probabilityInput == null ||
            string.IsNullOrEmpty(prizeUI.probabilityInput.text) ||
            !int.TryParse(prizeUI.probabilityInput.text, out int prob) ||
            prob < 0)
            return false;

        // Check quantity dropdown has selection
        if (prizeUI.quantityDropdown == null || prizeUI.quantityDropdown.value < 0)
            return false;

        return true;
    }

    /// <summary>
    /// Saves data for Close button functionality
    /// </summary>
    private void SaveCloseData()
    {
        PrizeCollection collection = new PrizeCollection();

        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);
            collection.prizes.Add(prize);
        }

        string jsonData = JsonUtility.ToJson(collection);
        PlayerPrefs.SetString(closeStorageKey, jsonData);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves current state for Save button functionality
    /// </summary>
    private void SaveCurrentState()
    {
        PrizeCollection collection = new PrizeCollection();

        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);
            collection.prizes.Add(prize);
        }

        string jsonData = JsonUtility.ToJson(collection);
        PlayerPrefs.SetString(saveStorageKey, jsonData);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Creates Prize from UI components
    /// </summary>
    /// <param name="prizeUI">UI component reference</param>
    /// <returns>Populated Prize object</returns>
    private Prize CreatePrizeFromUI(PrizeObjectUI prizeUI)
    {
        return new Prize
        {
            slot_id = prizeUI.slotID,
            name = SlotIDToName(prizeUI.slotID),
            showName = prizeUI.nameToShowDropdown?.options[prizeUI.nameToShowDropdown.value]?.text ?? "",
            price = 0f,
            rewardName = "Reward Name",
            probabilityWeight = int.TryParse(prizeUI.probabilityInput?.text, out int prob) ? prob : 0,
            inStock = prizeUI.inStockToggle?.isOn ?? false,
            quantity = prizeUI.quantityDropdown?.value ?? 0
        };
    }

    /// <summary>
    /// Loads data for Close button functionality
    /// </summary>
    private void LoadCloseData()
    {
        if (PlayerPrefs.HasKey(closeStorageKey))
        {
            string jsonData = PlayerPrefs.GetString(closeStorageKey);
            PrizeCollection collection = JsonUtility.FromJson<PrizeCollection>(jsonData);
            ApplyPrizeDataToUI(collection);
        }
    }

    /// <summary>
    /// Loads saved state for Load button functionality
    /// </summary>
    private void LoadSavedState()
    {
        if (PlayerPrefs.HasKey(saveStorageKey))
        {
            string jsonData = PlayerPrefs.GetString(saveStorageKey);
            PrizeCollection collection = JsonUtility.FromJson<PrizeCollection>(jsonData);
            ApplyPrizeDataToUI(collection);
        }
        else
        {
            Debug.LogWarning("No saved state found");
        }
    }

    /// <summary>
    /// Applies prize data to UI components
    /// </summary>
    /// <param name="collection">Prize data collection to apply</param>
    private void ApplyPrizeDataToUI(PrizeCollection collection)
    {
        foreach (var prize in collection.prizes)
        {
            var prizeUI = prizeUIs.Find(p => p.slotID == prize.slot_id);
            if (prizeUI != null)
            {
                // Apply nameToShow dropdown
                if (prizeUI.nameToShowDropdown != null)
                {
                    int nameIndex = prizeUI.nameToShowDropdown.options.FindIndex(option => option.text == prize.showName);
                    prizeUI.nameToShowDropdown.value = nameIndex >= 0 ? nameIndex : 0;
                    prizeUI.nameToShowDropdown.RefreshShownValue();
                }

                // Apply probability input
                if (prizeUI.probabilityInput != null)
                {
                    prizeUI.probabilityInput.text = prize.probabilityWeight.ToString();
                }

                // Apply inStock toggle
                if (prizeUI.inStockToggle != null)
                {
                    prizeUI.inStockToggle.isOn = prize.inStock;
                }

                // Apply quantity dropdown
                if (prizeUI.quantityDropdown != null)
                {
                    prizeUI.quantityDropdown.value = Mathf.Clamp(prize.quantity, 0, 10);
                    prizeUI.quantityDropdown.RefreshShownValue();
                }
            }
        }
    }

    /// <summary>
    /// Gets all prize data as InfoPrize array
    /// </summary>
    /// <returns>Array of InfoPrize objects</returns>
    public InfoPrize[] GetInfoPrizes()
    {
        List<InfoPrize> infoPrizes = new List<InfoPrize>();

        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);

            InfoPrize infoPrize = new InfoPrize
            {
                machine_id = "machine_id_placeholder",
                event_id = "event_id_placeholder",
                prize = prize
            };

            infoPrizes.Add(infoPrize);
        }

        return infoPrizes.ToArray();
    }

    /// <summary>
    /// Gets the current list of prize names
    /// </summary>
    /// <returns>List of prize names</returns>
    public List<string> GetPrizeNames()
    {
        return new List<string>(prizeNames);
    }

    /// <summary>
    /// Clears all saved prize names
    /// </summary>
    public void ClearAllPrizeNames()
    {
        prizeNames.Clear();
        PlayerPrefs.DeleteKey(offlinePrizesKey);
        PlayerPrefs.Save();

        UpdateEraseDropdown();
        UpdateAllPrizeDropdowns();

        Debug.Log("All prize names have been cleared");
    }

    /// <summary>
    /// Gets a random prize and decrements its quantity, then saves the state
    /// </summary>
    /// <returns>Selected Prize object or null if no available prizes</returns>
    public Prize GetRandomPrizeAndDecrement()
    {
        Prize selectedPrize = GetRandomPrize();

        if (selectedPrize != null)
        {
            // Find the UI element and decrement quantity
            var prizeUI = prizeUIs.Find(p => p.slotID == selectedPrize.slot_id);
            if (prizeUI != null && prizeUI.quantityDropdown != null)
            {
                int newQuantity = prizeUI.quantityDropdown.value - 1;
                if (newQuantity >= 0)
                {
                    prizeUI.quantityDropdown.value = newQuantity;
                    prizeUI.quantityDropdown.RefreshShownValue();

                    // Update the prize quantity
                    selectedPrize.quantity = newQuantity;

                    // Save the updated state to both Close and Save storage
                    SaveCloseData();
                    SaveCurrentState();

                    Debug.Log($"Decremented quantity for {selectedPrize.showName}. New quantity: {newQuantity}");
                }
            }
        }

        return selectedPrize;
    }

    /// <summary>
    /// Decrements the quantity of a specific prize by slot ID and saves the state
    /// </summary>
    /// <param name="slotID">Slot ID of the prize to decrement</param>
    /// <returns>True if successful, false if prize not found or quantity already zero</returns>
    public bool DecrementPrizeQuantity(string slotID)
    {
        var prizeUI = prizeUIs.Find(p => p.slotID == slotID);
        if (prizeUI != null && prizeUI.quantityDropdown != null)
        {
            int newQuantity = prizeUI.quantityDropdown.value - 1;
            if (newQuantity >= 0)
            {
                prizeUI.quantityDropdown.value = newQuantity;
                prizeUI.quantityDropdown.RefreshShownValue();

                // Save the updated state to both Close and Save storage
                SaveCloseData();
                SaveCurrentState();

                Debug.Log($"Decremented quantity for prize {slotID}. New quantity: {newQuantity}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot decrement quantity for prize {slotID}. Quantity is already zero.");
            }
        }
        else
        {
            Debug.LogError($"Prize with slot ID {slotID} not found");
        }

        return false;
    }

    /// <summary>
    /// Updates the quantity of a specific prize and saves the state
    /// </summary>
    /// <param name="slotID">Slot ID of the prize</param>
    /// <param name="newQuantity">New quantity value</param>
    /// <returns>True if successful</returns>
    public bool UpdatePrizeQuantity(string slotID, int newQuantity)
    {
        var prizeUI = prizeUIs.Find(p => p.slotID == slotID);
        if (prizeUI != null && prizeUI.quantityDropdown != null)
        {
            newQuantity = Mathf.Clamp(newQuantity, 0, 10);
            prizeUI.quantityDropdown.value = newQuantity;
            prizeUI.quantityDropdown.RefreshShownValue();

            // Save the updated state to both Close and Save storage
            SaveCloseData();
            SaveCurrentState();

            Debug.Log($"Updated quantity for prize {slotID} to {newQuantity}");
            return true;
        }

        Debug.LogError($"Prize with slot ID {slotID} not found");
        return false;
    }

    /// <summary>
    /// Updates the stock status of a specific prize and saves the state
    /// </summary>
    /// <param name="slotID">Slot ID of the prize</param>
    /// <param name="inStock">New stock status</param>
    /// <returns>True if successful</returns>
    public bool UpdatePrizeStock(string slotID, bool inStock)
    {
        var prizeUI = prizeUIs.Find(p => p.slotID == slotID);
        if (prizeUI != null && prizeUI.inStockToggle != null)
        {
            prizeUI.inStockToggle.isOn = inStock;

            // Save the updated state to both Close and Save storage
            SaveCloseData();
            SaveCurrentState();

            Debug.Log($"Updated stock status for prize {slotID} to {inStock}");
            return true;
        }

        Debug.LogError($"Prize with slot ID {slotID} not found");
        return false;
    }

    /// <summary>
    /// Saves the current state of all prizes (convenience method)
    /// </summary>
    public void SaveCurrentPrizeState()
    {
        SaveCloseData();
        SaveCurrentState();
        Debug.Log("Prize state saved successfully");
    }

    /// <summary>
    /// Gets a random prize based on probability weights and availability
    /// </summary>
    /// <returns>Selected Prize object or null if no available prizes</returns>
    public Prize GetRandomPrize()
    {
        List<Prize> availablePrizes = new List<Prize>();
        List<int> probabilities = new List<int>();
        int totalProbability = 0;

        // Collect all available prizes with quantity > 0 and inStock = true
        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);

            if (prize.inStock && prize.quantity > 0 && prize.probabilityWeight > 0)
            {
                availablePrizes.Add(prize);
                probabilities.Add(prize.probabilityWeight);
                totalProbability += prize.probabilityWeight;
            }
        }

        // Check if there are any available prizes
        if (availablePrizes.Count == 0)
        {
            Debug.LogWarning("No available prizes with quantity > 0 and inStock = true");
            return null;
        }

        // If only one prize available, return it
        if (availablePrizes.Count == 1)
        {
            Debug.Log($"Only one prize available: {availablePrizes[0].showName}");
            return availablePrizes[0];
        }

        // Generate random number based on total probability
        int randomValue = Random.Range(0, totalProbability);
        int cumulativeProbability = 0;

        // Select prize based on probability weights
        for (int i = 0; i < availablePrizes.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability)
            {
                Debug.Log($"Selected prize: {availablePrizes[i].showName} with probability {probabilities[i]}/{totalProbability}");
                return availablePrizes[i];
            }
        }

        // Fallback - return the last prize (should not reach here)
        Debug.LogWarning("Probability selection fallback - returning last prize");
        return availablePrizes[availablePrizes.Count - 1];
    }

    /// <summary>
    /// Gets all available prizes with quantity > 0 and inStock = true
    /// </summary>
    /// <returns>List of available prizes</returns>
    public List<Prize> GetAvailablePrizes()
    {
        List<Prize> availablePrizes = new List<Prize>();

        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);

            if (prize.inStock && prize.quantity > 0)
            {
                availablePrizes.Add(prize);
            }
        }

        return availablePrizes;
    }

    /// <summary>
    /// Gets the total probability weight of all available prizes
    /// </summary>
    /// <returns>Total probability weight</returns>
    public int GetTotalProbability()
    {
        int totalProbability = 0;

        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);

            if (prize.inStock && prize.quantity > 0)
            {
                totalProbability += prize.probabilityWeight;
            }
        }

        return totalProbability;
    }

    /// <summary>
    /// Checks if there are any available prizes
    /// </summary>
    /// <returns>True if there are available prizes</returns>
    public bool HasAvailablePrizes()
    {
        foreach (var prizeUI in prizeUIs)
        {
            Prize prize = CreatePrizeFromUI(prizeUI);

            if (prize.inStock && prize.quantity > 0)
            {
                return true;
            }
        }

        return false;
    }
}