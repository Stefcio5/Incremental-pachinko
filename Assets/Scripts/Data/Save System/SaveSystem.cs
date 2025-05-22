
public class SaveSystem
{
    private readonly IDataRepository _dataRepository;

    public SaveSystem(IDataRepository repository)
    {
        _dataRepository = repository;
    }

    public void Save(GameData data)
    {
        _dataRepository.Save(data);
    }

    public GameData Load()
    {
        return _dataRepository.Load();
    }
}