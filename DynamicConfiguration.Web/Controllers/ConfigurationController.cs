using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Common.Events;
using DynamicConfiguration.Web.Configuration.Interfaces;
using DynamicConfiguration.Web.Configuration.Models;
using DynamicConfiguration.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicConfiguration.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationStore _store;
        private readonly IConfigurationEventPublisher _eventPublisher;

        public ConfigurationController(
            IConfigurationStore store,
            IConfigurationEventPublisher eventPublisher)
        {
            _store = store;
            _eventPublisher = eventPublisher;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = (await _store.GetAllAsync(cancellationToken))
                .Select(c => new ConfigurationViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Value = c.Value,
                    IsActive = c.IsActive,
                    ApplicationName = c.ApplicationName
                })
                .ToList();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new ConfigurationViewModel { IsActive = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConfigurationViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(vm);
            var entity = new ConfigurationItem
            {
                Name = vm.Name,
                Type = vm.Type,
                Value = vm.Value,
                IsActive = vm.IsActive,
                ApplicationName = vm.ApplicationName
            };

            await _store.AddAsync(entity, cancellationToken);
            
            await _eventPublisher.ConfigurationChangedEventPublish(new ConfigurationChangedEvent
            {
                ApplicationName = vm.ApplicationName,
                Action = "Create",
                Timestamp = DateTime.Now
            }, cancellationToken);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var c = await _store.GetByIdAsync(id);
            var vm = new ConfigurationViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Value = c.Value,
                IsActive = c.IsActive,
                ApplicationName = c.ApplicationName,
                Version = c.Version
            };
            
            if (TempData.ContainsKey("ConcurrencyError"))
            {
                ModelState.AddModelError(
                    string.Empty,
                    TempData["ConcurrencyError"].ToString());
            }
            
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ConfigurationViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(vm);

            var entity = new ConfigurationItem
            {
                Id = vm.Id,
                Name = vm.Name,
                Type = vm.Type,
                Value = vm.Value,
                IsActive = vm.IsActive,
                ApplicationName = vm.ApplicationName,
                Version = vm.Version
            };

            var updateResult = await _store.UpdateAsync(vm.Id, entity, cancellationToken);

            if (!updateResult)
            {
                TempData["ConcurrencyError"] =
                    "Bu kayıt başka biri tarafından güncellendi. Lütfen tekrar deneyin.";
                
                return RedirectToAction(nameof(Edit), new { id = vm.Id });
            }
            
            await _eventPublisher.ConfigurationChangedEventPublish(new ConfigurationChangedEvent
            {
                ApplicationName = vm.ApplicationName,
                Action = "Edit",
                Timestamp = DateTime.Now
            }, cancellationToken);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var configurationItem = await _store.GetByIdAsync(id, cancellationToken);

            if (configurationItem == null)
            {
                return NotFound();
            }
            
            var deleteResult = await _store.DeleteAsync(id, cancellationToken);

            if (!deleteResult)
            {
                return Conflict("Bu kayıt başka biri tarafından güncellendi lütfen tekrar deneyin."); 
            }
            
            await _eventPublisher.ConfigurationChangedEventPublish(new ConfigurationChangedEvent
            {
                ApplicationName = configurationItem.ApplicationName,
                Action = "Edit",
                Timestamp = DateTime.Now
            }, cancellationToken);
            
            return RedirectToAction(nameof(Index));
        }
    }
}