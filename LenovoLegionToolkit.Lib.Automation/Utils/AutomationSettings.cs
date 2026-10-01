using System.Collections.Generic;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Automation.Utils;

public class AutomationSettings() : AbstractSettings<AutomationSettings.AutomationSettingsStore>("automation.json")
{
    public class AutomationSettingsStore
    {
        public bool IsEnabled { get; set; }

        public List<AutomationPipeline> Pipelines { get; set; } = [];

        // When true, AC adapter connect/disconnect events are debounced before
        // reaching pipelines, suppressing spurious rapid oscillation (e.g. faulty
        // AC telemetry reporting 1-3s false disconnect/reconnect pulses).
        public bool IsPowerAdapterDebounceEnabled { get; set; } = true;

        // Seconds the adapter state must remain stable before an adapter
        // connect/disconnect event is forwarded to pipelines.
        // 0 = re-validate state immediately without waiting.
        public int PowerAdapterDebounceSeconds { get; set; } = 5;
    }

    protected override AutomationSettingsStore Default => new()
    {
        Pipelines =
        {
            new AutomationPipeline
            {
                Trigger = new ACAdapterConnectedAutomationPipelineTrigger(),
                Steps = { new PowerModeAutomationStep(PowerModeState.Balance) },
            },
            new AutomationPipeline
            {
                Trigger = new ACAdapterDisconnectedAutomationPipelineTrigger(),
                Steps = { new PowerModeAutomationStep(PowerModeState.Quiet) },
            },
            new AutomationPipeline
            {
                Name = Resource.DeactivateGpuQuickAction_Title,
                Steps = { new DeactivateGPUAutomationStep(DeactivateGPUAutomationStepState.KillApps) },
            },
        },
    };
}
