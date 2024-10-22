import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { TabViewModule } from "primeng/tabview";

import { GridModule } from "@features/grid";
import { AppConfigurationComponent } from "./app-configuration.component";
import { IpAddressConstraintService } from "./ip-address-constraint.service";
import { IpAddressConstraintsComponent } from "./components/ip-address-constraints.component";
import { CorePermissions } from "../../static-data";
import { RightsRequirementGuard } from "../../../../guards";


@NgModule({
	declarations: [ AppConfigurationComponent, IpAddressConstraintsComponent ],
	imports: [
		GridModule,

		TabViewModule,

		RouterModule.forChild([
			{
				path: "",
				component: AppConfigurationComponent,
				data: { permissions: CorePermissions.ManageAppConfiguration },
				canActivate: [ RightsRequirementGuard ],
			}
		])
	],
	providers: [ IpAddressConstraintService ]
})
export class AppConfigurationModule {}