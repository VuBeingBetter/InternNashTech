import { Plus, Trash2 } from "lucide-react";
import { useFieldArray } from "react-hook-form";
import type { Control } from "react-hook-form";
import { useEffect } from "react";
import type { ProductFormValues } from "../../components/common/ProductFormValues";

interface Props {
    control: Control<ProductFormValues>;
    name: string;
    initialJson?: Record<string, string>;
}

const JsonSpecEditor = ({ control, name, initialJson }: Props) => {
    const { fields, append, remove, replace } = useFieldArray({ control, name: name as "specFields" });

    useEffect(() => {
        if (initialJson && Object.keys(initialJson).length > 0 && fields.length === 0) {
            try {
                const formatted = Object.entries(initialJson).map(([key, value]) => ({ key, value: String(value) }));
                replace(formatted);
            } catch (error) {
                console.error("Failed to parse initial JSON: ", error);
            }
        }
    }, [initialJson, replace, fields.length]);

    return (
        <div className="space-y-3">
            <div className="bg-slate-50 p-4 rounded-xl border border-slate-200">
                {/* Header */}
                <div className="grid grid-cols-12 gap-4 mb-2 px-2">
                    <div className="col-span-4 text-xs font-bold text-slate-700 uppercase tracking-wide">Components</div>
                    <div className="col-span-7 text-xs font-bold text-slate-700 uppercase tracking-wide">Details</div>
                    <div className="col-span-1"></div>
                </div>

                {/* Spec fields */}
                <div className="space-y-2">
                    {fields.map((field, index) => (
                        <div key={field.id} className="grid grid-cols-12 gap-4 items-center animate-in fade-in slide-in-from-top-1">
                            {/* Component's name */}
                            <div className="col-span-4">
                                <input 
                                    {...control.register(`specFields.${index}.key` as const)}
                                    placeholder="e.g. CPU"
                                    className="w-full p-2 text-sm border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-slate-500"
                                />
                            </div>

                            {/* Details */}
                            <div className="col-span-7">
                                <input 
                                    {...control.register(`specFields.${index}.value` as const)}
                                    placeholder="e.g. Intel Core i7-14650H"
                                    className="w-full p-2 text-sm border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none text-slate-500"
                                />
                            </div>

                            {/* Delete button */}
                            <div className="col-span-1 flex justify-end">
                                <button
                                    title="Remove specification"
                                    type="button"
                                    onClick={() => remove(index)}
                                    className="p-2 text-slate-400 hover:text-red-600 transition-colors"
                                >
                                    <Trash2 size={18} />
                                </button>
                            </div>
                        </div>
                    ))}

                    {/* Add new spec button */}
                    <div className="flex justify-end mt-2">
                        <button
                            type="button"
                            onClick={() => append({ key: "", value: "" })}
                            className="mt-4 flex items-center justify-center w-full py-2 border-2 border-dashed border-slate-300 rounded-lg text-slate-500 hover:border-blue-400 hover:text-blue-500 hover:bg-blue-50 transition-all text-sm font-medium"
                        >
                            <Plus size={20} /> Add Specification
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default JsonSpecEditor;