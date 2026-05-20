using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class ElementSpritePair
{
    public ElementType element;
    public Sprite sprite;
}

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 7;
    [SerializeField] private float cellSize = 1f;

    [Header("Gem Settings")]
    [SerializeField] private GemView gemPrefab;
    [SerializeField] private List<ElementSpritePair> elementSprites = new();

    [Header("Energy System")]
    [SerializeField] private ElementEnergySystem alphaEnergySystem;

    [Header("Turn / Swap System")]
    [SerializeField] private SwapCounterSystem alphaSwapCounter;

    [Header("Turn Manager")]
    [SerializeField] private TurnManager turnManager;   

    [Header("Animation Settings")]
    [SerializeField] private float swapDuration = 0.15f;
    [SerializeField] private float fallDuration = 0.12f;
    [SerializeField] private float destroyDelay = 0.08f;

    private GemView[,] board;
    private GemView selectedGem;
    private bool isBusy;
    private Camera mainCamera;

    private struct MoveData
    {
        public GemView gem;
        public Vector3 startPosition;
        public Vector3 endPosition;

        public MoveData(GemView gem, Vector3 startPosition, Vector3 endPosition)
        {
            this.gem = gem;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy kiểm tra Camera có tag 'MainCamera' chưa.");
        }
    }

    private void Start()
    {
        board = new GemView[width, height];
        CreateBoard();
    }

    private void Update()
    {
        if (isBusy) return;

        if (Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, 0f)
            );

            Vector2 mousePoint = new Vector2(
                mouseWorldPosition.x,
                mouseWorldPosition.y
            );

            Collider2D hit = Physics2D.OverlapPoint(mousePoint);

            if (hit == null) return;

            GemView gem = hit.GetComponent<GemView>();

            if (gem == null) return;

            SelectGem(gem);
        }
    }

    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ElementType element = GetRandomElementAvoidingInitialMatch(x, y);
                CreateGem(x, y, element, GridToWorldPosition(x, y));
            }
        }
    }

    private GemView CreateGem(int x, int y, ElementType element, Vector3 spawnPosition)
    {
        GemView gem = Instantiate(gemPrefab, spawnPosition, Quaternion.identity, transform);
        Sprite sprite = GetSpriteByElement(element);

        gem.Init(x, y, element, this, sprite);
        board[x, y] = gem;

        return gem;
    }

    private ElementType GetRandomElement()
    {
        if (elementSprites == null || elementSprites.Count == 0)
        {
            throw new InvalidOperationException("Chưa gán Element Sprites trong BoardManager.");
        }

        int randomIndex = UnityEngine.Random.Range(0, elementSprites.Count);
        return elementSprites[randomIndex].element;
    }

    private ElementType GetRandomElementAvoidingInitialMatch(int x, int y)
    {
        for (int i = 0; i < 100; i++)
        {
            ElementType element = GetRandomElement();

            if (!WouldCreateInitialMatch(x, y, element))
            {
                return element;
            }
        }

        return GetRandomElement();
    }

    private bool WouldCreateInitialMatch(int x, int y, ElementType element)
    {
        bool horizontalMatch =
            x >= 2 &&
            board[x - 1, y] != null &&
            board[x - 2, y] != null &&
            board[x - 1, y].Element.Equals(element) &&
            board[x - 2, y].Element.Equals(element);

        bool verticalMatch =
            y >= 2 &&
            board[x, y - 1] != null &&
            board[x, y - 2] != null &&
            board[x, y - 1].Element.Equals(element) &&
            board[x, y - 2].Element.Equals(element);

        return horizontalMatch || verticalMatch;
    }

    private Sprite GetSpriteByElement(ElementType element)
    {
        foreach (ElementSpritePair pair in elementSprites)
        {
            if (pair.element.Equals(element))
            {
                return pair.sprite;
            }
        }

        Debug.LogError($"Không tìm thấy sprite cho element: {element}");
        return null;
    }

    private Vector3 GridToWorldPosition(int x, int y)
    {
        float offsetX = -(width - 1) * cellSize / 2f;
        float offsetY = -(height - 1) * cellSize / 2f;

        return transform.position + new Vector3(
            offsetX + x * cellSize,
            offsetY + y * cellSize,
            0f
        );
    }

    public void SelectGem(GemView gem)
    {
        if (isBusy || gem == null) return;

        if(turnManager != null && !turnManager.CanAlphaSwap())
        {
            Debug.Log("Khong the Swap. Hien khong phai Alpha Swap Phase.");
            return;
        }

        if (alphaEnergySystem != null && !alphaSwapCounter.HasSwapLeft())
        {
            Debug.Log("Không thể chọn đá quý. Hết lượt swap.");
            return;
        }

        if (selectedGem == null)
        {
            selectedGem = gem;
            selectedGem.SetSelected(true);
            return;
        }

        if (selectedGem == gem)
        {
            selectedGem.SetSelected(false);
            selectedGem = null;
            return;
        }

        if (AreAdjacent(selectedGem, gem))
        {
            StartCoroutine(TrySwapGems(selectedGem, gem));
        }
        else
        {
            selectedGem.SetSelected(false);

            selectedGem = gem;
            selectedGem.SetSelected(true);
        }
    }

    private int GetLargestMatchedGroupSize(List<GemView> matches)
    {
        HashSet<GemView> matchSet = new HashSet<GemView>(matches);
        HashSet<GemView> visited = new HashSet<GemView>();

        int largestGroupSize = 0;

        foreach (GemView gem in matches)
        {
            if (gem == null) continue;
            if (visited.Contains(gem)) continue;

            int groupSize = CountConnectedMatchedGroup(gem, matchSet, visited);

            largestGroupSize = Mathf.Max(largestGroupSize, groupSize);
        }

        Debug.Log($"Largest matched group size: {largestGroupSize}");

        return largestGroupSize;
    }

    private int CountConnectedMatchedGroup(
    GemView startGem,
    HashSet<GemView> matchSet,
    HashSet<GemView> visited
)
    {
        int count = 0;
        ElementType targetElement = startGem.Element;

        Queue<GemView> queue = new Queue<GemView>();
        queue.Enqueue(startGem);
        visited.Add(startGem);

        while (queue.Count > 0)
        {
            GemView current = queue.Dequeue();
            count++;

            foreach (GemView neighbor in GetMatchedNeighbors(current, matchSet))
            {
                if (neighbor == null) continue;
                if (visited.Contains(neighbor)) continue;
                if (!neighbor.Element.Equals(targetElement)) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        return count;
    }

    private IEnumerable<GemView> GetMatchedNeighbors(GemView gem, HashSet<GemView> matchSet)
    {
        int x = gem.X;
        int y = gem.Y;

        if (x > 0 && board[x - 1, y] != null && matchSet.Contains(board[x - 1, y]))
        {
            yield return board[x - 1, y];
        }

        if (x < width - 1 && board[x + 1, y] != null && matchSet.Contains(board[x + 1, y]))
        {
            yield return board[x + 1, y];
        }

        if (y > 0 && board[x, y - 1] != null && matchSet.Contains(board[x, y - 1]))
        {
            yield return board[x, y - 1];
        }

        if (y < height - 1 && board[x, y + 1] != null && matchSet.Contains(board[x, y + 1]))
        {
            yield return board[x, y + 1];
        }
    }

    private void ApplySwapruleByMatchLength(int highestMatchLength)
    {
        if(alphaSwapCounter == null)
        {
            Debug.LogWarning("Swap Counter System chưa được gán trong BoardManager. Không thể áp dụng quy tắc swap.");
            return;
        }

        if(highestMatchLength >= 5)
        {
            alphaSwapCounter.AddSwap(2);
        }
        else if (highestMatchLength == 4)
        {
            alphaSwapCounter.AddSwap(1);
        }
        else
        {
            Debug.Log("Không có quy tắc swap nào được áp dụng cho lượt swap này.");
        }
    }

    private bool AreAdjacent(GemView a, GemView b)
    {
        int deltaX = Mathf.Abs(a.X - b.X);
        int deltaY = Mathf.Abs(a.Y - b.Y);

        return deltaX + deltaY == 1;
    }

    private IEnumerator TrySwapGems(GemView a, GemView b)
    {
        isBusy = true;

        a.SetSelected(false);
        b.SetSelected(false);
        selectedGem = null;

        yield return SwapGems(a, b);

        List<GemView> matches = FindAllMatches();

        if (matches.Count == 0)
        {
            yield return SwapGems(a, b);
            isBusy = false;
            yield break;
        }

        alphaSwapCounter?.SpendOneSwap();

        int highestMatchLength = GetLargestMatchedGroupSize(matches);
        ApplySwapruleByMatchLength(highestMatchLength);

        yield return ResolveBoard();

        isBusy = false;
    }

    private IEnumerator SwapGems(GemView a, GemView b)
    {
        int ax = a.X;
        int ay = a.Y;
        int bx = b.X;
        int by = b.Y;

        board[ax, ay] = b;
        board[bx, by] = a;

        a.SetGridPosition(bx, by);
        b.SetGridPosition(ax, ay);

        Vector3 aStart = a.transform.position;
        Vector3 bStart = b.transform.position;

        Vector3 aEnd = GridToWorldPosition(a.X, a.Y);
        Vector3 bEnd = GridToWorldPosition(b.X, b.Y);

        float timer = 0f;

        while (timer < swapDuration)
        {
            timer += Time.deltaTime;
            float t = timer / swapDuration;

            a.transform.position = Vector3.Lerp(aStart, aEnd, t);
            b.transform.position = Vector3.Lerp(bStart, bEnd, t);

            yield return null;
        }

        a.transform.position = aEnd;
        b.transform.position = bEnd;
    }

    private IEnumerator ResolveBoard()
    {
        while (true)
        {
            List<GemView> matches = FindAllMatches();

            if (matches.Count == 0)
            {
                yield break;
            }

            ClearMatches(matches);

            yield return new WaitForSeconds(destroyDelay);

            yield return CollapseColumns();
            yield return FillEmptySpaces();
        }
    }

    private List<GemView> FindAllMatches()
    {
        HashSet<GemView> matchedGems = new HashSet<GemView>();

        FindHorizontalMatches(matchedGems);
        FindVerticalMatches(matchedGems);

        return new List<GemView>(matchedGems);
    }

    private void FindHorizontalMatches(HashSet<GemView> matchedGems)
    {
        for (int y = 0; y < height; y++)
        {
            int matchStartX = 0;
            int matchLength = 1;

            for (int x = 1; x < width; x++)
            {
                GemView current = board[x, y];
                GemView previous = board[x - 1, y];

                bool sameElement =
                    current != null &&
                    previous != null &&
                    current.Element.Equals(previous.Element);

                if (sameElement)
                {
                    matchLength++;
                }
                else
                {
                    AddLineMatchIfValid(matchedGems, matchStartX, y, matchLength, true);

                    matchStartX = x;
                    matchLength = 1;
                }
            }

            AddLineMatchIfValid(matchedGems, matchStartX, y, matchLength, true);
        }
    }

    private void FindVerticalMatches(HashSet<GemView> matchedGems)
    {
        for (int x = 0; x < width; x++)
        {
            int matchStartY = 0;
            int matchLength = 1;

            for (int y = 1; y < height; y++)
            {
                GemView current = board[x, y];
                GemView previous = board[x, y - 1];

                bool sameElement =
                    current != null &&
                    previous != null &&
                    current.Element.Equals(previous.Element);

                if (sameElement)
                {
                    matchLength++;
                }
                else
                {
                    AddLineMatchIfValid(matchedGems, x, matchStartY, matchLength, false);

                    matchStartY = y;
                    matchLength = 1;
                }
            }

            AddLineMatchIfValid(matchedGems, x, matchStartY, matchLength, false);
        }
    }

    private void AddLineMatchIfValid(
        HashSet<GemView> matchedGems,
        int startX,
        int startY,
        int length,
        bool horizontal
    )
    {
        if (length < 3) return;

        for (int i = 0; i < length; i++)
        {
            int x = horizontal ? startX + i : startX;
            int y = horizontal ? startY : startY + i;

            if (board[x, y] != null)
            {
                matchedGems.Add(board[x, y]);
            }
        }
    }

    private void ClearMatches(List<GemView> matches)
    {
        Dictionary<ElementType, int> matchedElementCounts = new();


        foreach (GemView gem in matches)
        {
            if (gem == null) continue;

            if (!matchedElementCounts.ContainsKey(gem.Element))
            {
                matchedElementCounts.Add(gem.Element, 0);
            }

            matchedElementCounts[gem.Element]++;
        }

        GiveEnergyFormMatches(matchedElementCounts);

        foreach(GemView gem in matches)
        {
            if (gem == null) continue;

            board[gem.X, gem.Y] = null;
            Destroy(gem.gameObject);
        }
    }

    private void GiveEnergyFormMatches(Dictionary<ElementType, int> matchedElementCounts)
    {
        if(alphaEnergySystem == null)
        {
            Debug.LogWarning("Alpha Energy System chưa được gán trong BoardManager. Không thể cấp năng lượng.");
            return;
        }

        foreach(KeyValuePair<ElementType, int> pair in matchedElementCounts)
        {
            ElementType element = pair.Key;
            int amount = pair.Value;

            alphaEnergySystem.AddEnergy(element, amount);
        }
    }

    private IEnumerator CollapseColumns()
    {
        List<MoveData> moves = new List<MoveData>();

        for (int x = 0; x < width; x++)
        {
            int targetY = 0;

            for (int y = 0; y < height; y++)
            {
                GemView gem = board[x, y];

                if (gem == null) continue;

                if (y != targetY)
                {
                    board[x, y] = null;
                    board[x, targetY] = gem;

                    Vector3 startPosition = gem.transform.position;

                    gem.SetGridPosition(x, targetY);

                    Vector3 endPosition = GridToWorldPosition(x, targetY);
                    moves.Add(new MoveData(gem, startPosition, endPosition));
                }

                targetY++;
            }
        }

        yield return MoveManyGems(moves, fallDuration);
    }

    private IEnumerator FillEmptySpaces()
    {
        List<MoveData> moves = new List<MoveData>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] != null) continue;

                ElementType element = GetRandomElement();

                Vector3 spawnPosition = GridToWorldPosition(x, y + height);
                GemView gem = CreateGem(x, y, element, spawnPosition);

                Vector3 endPosition = GridToWorldPosition(x, y);
                moves.Add(new MoveData(gem, spawnPosition, endPosition));
            }
        }

        yield return MoveManyGems(moves, fallDuration);
    }

    private IEnumerator MoveManyGems(List<MoveData> moves, float duration)
    {
        if (moves.Count == 0) yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            foreach (MoveData move in moves)
            {
                if (move.gem == null) continue;

                move.gem.transform.position = Vector3.Lerp(
                    move.startPosition,
                    move.endPosition,
                    t
                );
            }

            yield return null;
        }

        foreach (MoveData move in moves)
        {
            if (move.gem == null) continue;

            move.gem.transform.position = move.endPosition;
        }
    }
}