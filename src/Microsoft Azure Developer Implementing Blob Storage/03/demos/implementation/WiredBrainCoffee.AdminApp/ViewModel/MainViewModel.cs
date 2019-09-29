﻿using System.Threading.Tasks;
using WiredBrainCoffee.Storage;
using System;
using System.Collections.ObjectModel;
using WiredBrainCoffee.AdminApp.Service;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    private bool _isLoading;
    private string _loadingMessage;
    private readonly ICoffeeVideoStorage _coffeeVideoStorage;
    private readonly IAddCoffeeVideoDialogService _addCoffeeVideoDialogService;
    private readonly IMessageDialogService _messageDialogService;
    private CoffeeVideoViewModel _selectedCoffeeVideoViewModel;

    public MainViewModel(ICoffeeVideoStorage coffeeVideoStorage,
      IAddCoffeeVideoDialogService addCoffeeVideoDialogService,
      IMessageDialogService messageDialogService)
    {
      _coffeeVideoStorage = coffeeVideoStorage;
      _addCoffeeVideoDialogService = addCoffeeVideoDialogService;
      _messageDialogService = messageDialogService;
      CoffeeVideos = new ObservableCollection<CoffeeVideoViewModel>();
    }

    public bool IsLoading
    {
      get { return _isLoading; }
      set
      {
        _isLoading = value;
        OnPropertyChanged();
      }
    }

    public string LoadingMessage
    {
      get { return _loadingMessage; }
      set
      {
        _loadingMessage = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<CoffeeVideoViewModel> CoffeeVideos { get; }

    public CoffeeVideoViewModel SelectedCoffeeVideo
    {
      get { return _selectedCoffeeVideoViewModel; }
      set
      {
        if (_selectedCoffeeVideoViewModel != value)
        {
          _selectedCoffeeVideoViewModel = value;
          OnPropertyChanged();
          OnPropertyChanged(nameof(IsCoffeeVideoSelected));
        }
      }
    }

    public bool IsCoffeeVideoSelected => SelectedCoffeeVideo != null;

    public async Task AddCoffeeVideoAsync()
    {
      try
      {
        var dialogData = await _addCoffeeVideoDialogService.ShowDialogAsync();

        if (dialogData.DialogResultIsOk)
        {
          StartLoading($"Uploading your video {dialogData.BlobName}");

          await _coffeeVideoStorage.UploadVideoAsync(
              dialogData.BlobByteArray,
              dialogData.BlobName);

          // TODO: Initialize CoffeeVideoViewModel with uploaded data
          var coffeeVideoViewModel = new CoffeeVideoViewModel
          {
            BlobName = dialogData.BlobName,
            BlobUri = "The Blob URI"
          };
          CoffeeVideos.Add(coffeeVideoViewModel);
          SelectedCoffeeVideo = coffeeVideoViewModel;
        }
      }
      catch (Exception ex)
      {
        await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
      }
      finally
      {
        StopLoading();
      }
    }

    public void StartLoading(string message)
    {
      LoadingMessage = message;
      IsLoading = true;
    }

    public void StopLoading()
    {
      IsLoading = false;
      LoadingMessage = null;
    }
  }
}
