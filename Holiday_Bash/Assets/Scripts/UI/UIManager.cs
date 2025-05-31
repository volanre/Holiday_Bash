using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public PlayerInputActions inputActions;
    private InputAction pauseMenuToggle;
    private InputAction inventoryToggle;
    private InputAction gameOverlayToggle;
    private bool gamePaused = false;
    [SerializeField] public GameObject pauseMenuCanvas;
    [SerializeField] public GameObject inventoryCanvas;
    [SerializeField] public GameObject gameOverlayCanvas;
    [SerializeField] public GameObject deathScreenCanvas;

    void Awake()
    {
        if (inputActions == null) inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        pauseMenuToggle = inputActions.UI.PauseMenu;
        pauseMenuToggle.Enable();
        pauseMenuToggle.performed += PauseGame;

        inventoryToggle = inputActions.UI.Inventory;
        inventoryToggle.Enable();
        inventoryToggle.performed += ToggleInventory;

        gameOverlayToggle = inputActions.UI.GameOverlay;
        gameOverlayToggle.Enable();
        gameOverlayToggle.performed += ToggleGameOverlay;
    }

    private void OnDisable()
    {
        pauseMenuToggle.Disable();
        inventoryToggle.Disable();
        gameOverlayToggle.Disable();
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        if (deathScreenCanvas.activeSelf) return;
        PauseGame();
    }
    public void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0f;
        pauseMenuCanvas.SetActive(true);
        SetCursorState();
        return;
    }
    public void Unpause()
    {
        gamePaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        SetCursorState();
    }
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        if (gamePaused) return;
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
        SetCursorState();
        return;
    }
    private void ToggleGameOverlay(InputAction.CallbackContext context)
    {
        if (gamePaused) return;
        if (inventoryCanvas.activeSelf) return;
        gameOverlayCanvas.SetActive(!gameOverlayCanvas.activeSelf);
        return;
    }

    private void SetCursorState()
    {
        bool isOn = false;
        if (inventoryCanvas.activeSelf || pauseMenuCanvas.activeSelf || deathScreenCanvas.activeSelf)
        {
            isOn = true;
        }
        if (isOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void OnGameStart()
    {
        deathScreenCanvas.SetActive(false);
        gameOverlayCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        SetCursorState();
    }
    public void OnGameEnd()
    {
        deathScreenCanvas.SetActive(true);
        gameOverlayCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        SetCursorState();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SetCursorState();
        }
        else
        {
            PauseGame();
        }
    }

}
