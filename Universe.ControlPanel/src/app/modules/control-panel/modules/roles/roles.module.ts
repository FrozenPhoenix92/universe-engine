import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { RolesComponent } from "./roles.component";
import { CorePermissions } from "../../static-data";
import { RightsRequirementGuard } from "../../../../guards";


@NgModule({
	imports: [ RouterModule.forChild([
			{
				path: "",
				component: RolesComponent,
				data: { permissions: CorePermissions.ManageRole },
				canActivate: [ RightsRequirementGuard ],
			}
		])
	],
	declarations: [ RolesComponent ]
})
export class RolesModule {}